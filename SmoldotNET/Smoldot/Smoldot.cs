﻿using System.Text;
using Wasmtime;
using MemoryExtensions = SmoldotNET.WasmMemory.MemoryExtensions;

namespace SmoldotNET.Smoldot;

public class Smoldot
{
    private readonly ISmoldotWasmFunctions _wasmFunctions;
    private readonly ISmoldotWasmLinks _wasmLinks;

    private readonly IMemoryTable _memoryTable = new MemoryTable();
    private readonly ITimerMap _timerMap = new TimerMap();
    private readonly IConnectionManager _connectionManager = new ConnectionManager();

    private readonly Instance _instance;
    
    private bool _shouldAdvanceExecution = true;

    public Smoldot(string wasmBlobPath)
    {
        var engine = new Engine();
        var module = Module.FromFile(engine, wasmBlobPath);
        var linker = new Linker(engine);
        var store = new Store(engine);
        
        _wasmLinks = new SmoldotWasmLinks(_memoryTable, _timerMap, ref _shouldAdvanceExecution, _connectionManager);
        _wasmLinks.Link(linker);
        
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
            foreach (var (connectionId, reason) in _connectionManager.ResetConnections())
                _wasmFunctions.ConnectionReset(connectionId, _memoryTable.Allocate(Encoding.UTF8.GetBytes(reason)));

            foreach (var (connectionId, additionalCapacity) in _connectionManager.StreamCapacityUpdates())
                _wasmFunctions.StreamWriteableBytes(connectionId, 0, additionalCapacity);

            foreach (var (connectionId, data) in _connectionManager.IncomingData())
                _wasmFunctions.StreamMessage(connectionId, 0, _memoryTable.Allocate(data));

            foreach (var chain in _chains)
                TryAndGetResponse(chain);
            
            await Task.Delay(50, ct);
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

    public event RpcResponse OnRpcResponse;

    private void TryAndGetResponse(int chainId)
    {
        var ptr = _wasmFunctions.JsonRpcResponsesPeek(chainId);

        var memory = _instance.GetMemory(MemoryExtensions.MemoryName)!;
        var responsePtr = (uint)memory.ReadInt32(ptr);
        var responseLen = memory.ReadInt32(ptr + 4);

        if (responseLen == 0)
            return;

        var response = memory.ReadString(responsePtr, responseLen);
        OnRpcResponse?.Invoke(chainId, response);

        _wasmFunctions.JsonRpcPop(chainId);
    }
    
}