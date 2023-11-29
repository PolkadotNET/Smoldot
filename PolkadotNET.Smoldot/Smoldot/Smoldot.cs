using System.Text;
using PolkadotNET.Smoldot.WasmMemory.Structs;
using Wasmtime;
using MemoryExtensions = PolkadotNET.Smoldot.WasmMemory.MemoryExtensions;

namespace PolkadotNET.Smoldot.Smoldot;

public class Smoldot
{
    private readonly ISmoldotWasmFunctions _wasmFunctions;

    private readonly IMemoryTable _memoryTable = new MemoryTable();
    private readonly ITimerMap _timerMap = new TimerMap();
    private readonly IConnectionManager _connectionManager = new ConnectionManager();

    private readonly Instance _instance;
    
    private readonly bool _shouldAdvanceExecution = true;

    public Smoldot(string wasmBlobPath)
    {
        var engine = new Engine();
        var module = Module.FromFile(engine, wasmBlobPath);
        var linker = new Linker(engine);
        var store = new Store(engine);
        
        var wasmLinks = new SmoldotWasmLinks(_memoryTable, _timerMap, ref _shouldAdvanceExecution, _connectionManager);
        wasmLinks.Link(linker);
        
        _instance = linker.Instantiate(store, module);

        _wasmFunctions = new SmoldotWasmFunctions(_instance);
    }

    public async Task Run(CancellationToken ct)
    {
        _wasmFunctions.Init(5);

        while (!ct.IsCancellationRequested)
        {
            // advance execution
            if (_shouldAdvanceExecution)
                _wasmFunctions.AdvanceExecution();

            // timer
            if (_timerMap.HasElapsed())
                _wasmFunctions.TimerFinished();

            // connection manager
            var allocatedIds = new List<int>();
            foreach (var (connectionId, reason) in _connectionManager.ResetConnections())
            {
                var bufferId = _memoryTable.Allocate(Encoding.UTF8.GetBytes(reason));
                _wasmFunctions.ConnectionReset(connectionId, bufferId);
                allocatedIds.Add(bufferId);
            }
                

            foreach (var (connectionId, additionalCapacity) in _connectionManager.StreamCapacityUpdates())
                _wasmFunctions.StreamWriteableBytes(connectionId, 0, additionalCapacity);

            foreach (var (connectionId, data) in _connectionManager.IncomingData())
            {
                var bufferId = _memoryTable.Allocate(data);
                _wasmFunctions.StreamMessage(connectionId, 0, bufferId);
                allocatedIds.Add(bufferId);
            }

            foreach (var chain in _chains)
                TryAndGetResponse(chain);
            
            foreach (var allocatedId in allocatedIds)
            {
                _memoryTable.Release(allocatedId);
            }
            
            await Task.Delay(10, ct);
        }
    }

    private readonly List<int> _chains = new();
    
    public Chain AddChain(string chainSpecification)
    {
        var spec = Encoding.UTF8.GetBytes(chainSpecification);
        var specBuffer = _memoryTable.Allocate(spec);
        var databaseContentBuffer = _memoryTable.Allocate(Array.Empty<byte>());
        var relayChainBuffer = _memoryTable.Allocate(Array.Empty<byte>());

        var chainId = _wasmFunctions.AddChain(specBuffer, databaseContentBuffer, 5, 5, relayChainBuffer);
        _chains.Add(chainId);
        return new Chain(this, chainId);
    }

    public int Send(int chainId, string message)
    {
        var data = Encoding.UTF8.GetBytes(message);
        var messageBuffer = _memoryTable.Allocate(data);

        var sendSuccess = _wasmFunctions.JsonRpcSend(messageBuffer, chainId);
        return sendSuccess;
    }
    
    public delegate void RpcResponse(int chainId, string response);

    public event RpcResponse? OnRpcResponse;

    private void TryAndGetResponse(int chainId)
    {
        var ptr = _wasmFunctions.JsonRpcResponsesPeek(chainId);
        var memory = _instance.GetMemory(MemoryExtensions.MemoryName)!;
        var responseInfo = memory.Read<JsonRpcResponseInfo>(ptr);

        if (responseInfo.Length == 0)
            return;

        var response = memory.ReadString(responseInfo.Pointer, responseInfo.Length);
        OnRpcResponse?.Invoke(chainId, response);

        _wasmFunctions.JsonRpcPop(chainId);
    }
    
}