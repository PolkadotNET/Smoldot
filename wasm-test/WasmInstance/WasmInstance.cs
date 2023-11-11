using System.Security.Cryptography;
using System.Text;
using NLog;
using Wasmtime;
using Timer = System.Timers.Timer;

namespace wasm_test.WasmInstance;

public class WasmInstance : IDisposable
{
    private readonly Linker _linker;
    private readonly Engine _engine;
    private readonly Module _module;
    private readonly Store _store;

    private readonly Instance _instance;

    private readonly Lazy<Action<int>> _init;
    private readonly Lazy<Action> _advanceExecution;
    private readonly Lazy<Action> _timerFinished;
    
    private readonly Lazy<Func<int, int, int, int, int, int>> _addChain;
    private readonly Lazy<Func<int, int>> _chainIsOk;
    
    private readonly Lazy<Func<int, int, int>> _jsonRpcSend;
    private readonly Lazy<Func<int, int>> _jsonRpcResponsesPeek;
    private readonly Lazy<Action<int>> _jsonRpcPop;
    
    private readonly long _startupTime;
    private bool _shouldAdvance = false;
    private readonly MemoryTable _memoryTable = new();
    private readonly TimerMap _timerMap = new();

    public WasmInstance()
    {
        _engine = new Engine();
        _module = Module.FromFile(_engine, "./smoldot_light_wasm.wasm");
        _linker = new Linker(_engine);
        _store = new Store(_engine);

        _startupTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _shouldAdvance = true;

        BindWasmEvents();
        _instance = _linker.Instantiate(_store, _module);

        _init = new Lazy<Action<int>>(_instance.GetAction<int>("init")!);
        _advanceExecution = new Lazy<Action>(_instance.GetAction("advance_execution")!);
        _timerFinished = new Lazy<Action>(_instance.GetAction("timer_finished")!);
        
        _addChain = new Lazy<Func<int, int, int, int, int, int>>(
            _instance.GetFunction<int, int, int, int, int, int>("add_chain")!
        );
        _chainIsOk = new Lazy<Func<int, int>>(_instance.GetFunction<int, int>("chain_is_ok")!);
        
        _jsonRpcSend = new Lazy<Func<int, int, int>>(_instance.GetFunction<int, int, int>("json_rpc_send")!);
        _jsonRpcResponsesPeek = new Lazy<Func<int, int>>(_instance.GetFunction<int, int>("json_rpc_responses_peek")!);
        _jsonRpcPop = new Lazy<Action<int>>(_instance.GetAction<int>("json_rpc_responses_pop")!);
    }

    private void BindWasmEvents()
    {
        // Smoldot Bindings
        _linker.BindRandomGet(OnRandomGet);
        _linker.BindLog(OnLog);
        _linker.BindPanic(OnPanic);
        _linker.BindBufferCopy(OnBufferCopy);
        _linker.BindBufferSize(OnBufferSize);
        _linker.BindConnectionNew(OnConnectionNew);
        _linker.BindResetConnection(OnResetConnection);
        _linker.BindStartTimer(OnStartTimer);
        _linker.BindStreamSend(OnStreamSend);
        _linker.BindAdvanceExecutionReady(OnAdvanceExecutionReady);
        _linker.BindConnectionStreamOpen(OnConnectionStreamOpen);
        _linker.BindConnectionStreamReset(OnConnectionStreamReset);
        _linker.BindConnectionTypeSupported(OnConnectionTypeSupported);
        _linker.BindCurrentTaskEntered((OnCurrentTaskEntered));
        _linker.BindCurrentTaskExit(OnCurrentTaskExit);
        _linker.BindStreamSendClose(OnStreamSendClose);
        _linker.BindJsonRpcResponseNonEmpty(OnJsonRpcResponseNonEmpty);
        _linker.BindUnixTimestampUs(OnUnixTimestampUs);
        _linker.BindMonotonicClockUs(OnMonotonicClockUs);
    }

    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private long OnUnixTimestampUs(Caller caller)
    {
        _logger.Trace("OnUnixTimestampUs");
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long OnMonotonicClockUs(Caller caller)
    {
        _logger.Trace("OnMonotonicCLockUs");
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _startupTime;
    }

    private void OnRandomGet(Caller caller, int targetPtr, int targetLen)
    {
        _logger.Trace("OnMonotonicCLockUs");
        caller.WriteByteArray(targetPtr, RandomNumberGenerator.GetBytes(targetLen));
    }

    private void OnAdvanceExecutionReady()
    {
        _logger.Trace("OnAdvanceExecutionReady");
        _shouldAdvance = true;
    }

    private void OnCurrentTaskEntered(Caller caller, int pointer, int length)
    {
        var task = caller.ReadStringFromMemory(pointer, length);
        _logger.Trace($"Task Entered: {task}");
    }

    private void OnCurrentTaskExit()
    {
        _logger.Trace("Current Task Exited");
    }

    private void OnConnectionNew(Caller caller, int id, int addressPointer, int addressLength)
    {
        _logger.Trace($"OnConnectionNew");
    }

    private void OnConnectionStreamOpen(int connectionId)
    {
        _logger.Trace($"Connection Stream {connectionId} opened");
    }

    private void OnConnectionStreamReset(int connectionId, int streamId)
    {
        _logger.Trace($"Connection {connectionId} Stream {streamId} reset");
    }

    private int OnConnectionTypeSupported(int connectionType)
    {
        const int No = 0;
        const int Yes = 1;

        _logger.Trace($"OnConnectionTypeSupported {connectionType}");
        return No;
    }

    private void OnJsonRpcResponseNonEmpty(int chainId)
    {
        _logger.Trace("OnJsonRpcResponseNonEmpty");
    }

    private void OnResetConnection(int connectionId)
    {
        _logger.Trace($"OnConnectionReset: {connectionId}");
    }

    private void OnStartTimer(double duration)
    {
        _timerMap.Add(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + duration);
        _logger.Trace($"OnStartTimer: {duration}");
    }

    private void OnStreamSend(int connectionId, int streamId, int ptr, int len)
    {
        _logger.Trace($"OnStreamSend");
    }

    private void OnStreamSendClose(int connectionId, int streamId)
    {
        _logger.Trace($"OnStreamSendClose");
    }

    private int OnBufferSize(int index)
    {
        _logger.Trace($"OnBufferSize");
        return _memoryTable.GetBuffer(index).Length;
    }

    private void OnBufferCopy(Caller caller, int bufferIndex, int targetPointer)
    {
        _logger.Trace($"Copying Buffer {bufferIndex} into Memory at {targetPointer}");
        caller.WriteByteArray(targetPointer, _memoryTable.GetBuffer(bufferIndex));
    }

    private void OnPanic(Caller caller, int messagePointer, int messageLength)
    {
        var error = caller.ReadStringFromMemory(messagePointer, messageLength);
        throw new Exception($"wasm paniced with '{error}'");
    }

    private void OnLog(Caller caller, int level, int targetPointer, int targetLength, int messagePointer,
        int messageLength)
    {
        var target = caller.ReadStringFromMemory(targetPointer, targetLength);
        var message = caller.ReadStringFromMemory(messagePointer, messageLength);

        _logger.Log(FromIntLevel(level), message);
    }

    private LogLevel FromIntLevel(int level)
    {
        return level switch
        {
            1 => LogLevel.Error,
            2 => LogLevel.Warn,
            3 => LogLevel.Info,
            4 => LogLevel.Debug,
            5 => LogLevel.Trace,
            _ => LogLevel.Info
        };
    }

    public async Task Run(CancellationToken ct)
    {
        _init.Value(5);

        while (!ct.IsCancellationRequested)
        {
            if (this._timerMap.HasElapsed())
            {
                _timerFinished.Value();
            }

            if (this._shouldAdvance)
            {
                _logger.Trace("advancing...");
                _shouldAdvance = false;
                _advanceExecution.Value();
            }
            
            foreach (var chain in _chains)
            {
                TryAndGetResponse(chain);
            }

            await Task.Delay(50, ct);
        }
    }

    private List<int> _chains = new();
    public int AddChain()
    {
        var spec = Encoding.UTF8.GetBytes(File.ReadAllText("./westend.json"));
        var specBuffer = _memoryTable.AllocateBuffer(spec);
        var databaseContentBuffer = _memoryTable.AllocateBuffer(Array.Empty<byte>());
        var relayChainBuffer = _memoryTable.AllocateBuffer(Array.Empty<byte>());

        var chainId =  _addChain.Value(specBuffer, databaseContentBuffer, 5, 5, relayChainBuffer);
        
        // check if chain is ok, else throw exception
        // var r = _chainIsOk.Value(chainId);
        _chains.Add(chainId);
        return chainId;
    }

    public int Send(int chainId, string message)
    {
        var data = Encoding.UTF8.GetBytes(message);
        var messageBuffer = _memoryTable.AllocateBuffer(data);
        
        var sendSuccess = _jsonRpcSend.Value(messageBuffer, chainId);
        return sendSuccess;
    }

    public delegate void RpcResponse(string response);

    public event RpcResponse OnRpcResponse;
    
    private void TryAndGetResponse(int chainId)
    {
        var ptr = _jsonRpcResponsesPeek.Value(chainId);

        var memory = _instance.GetMemory(WasmtimeExtensions.MemoryName)!;
        var responsePtr = (uint)memory.ReadInt32(ptr);
        var responseLen = memory.ReadInt32(ptr + 4);

        if (responseLen == 0)
            return;

        var response = memory.ReadString(responsePtr, responseLen);
        OnRpcResponse?.Invoke(response);

        _jsonRpcPop.Value(chainId);
    }

    public void Dispose()
    {
        _linker.Dispose();
        _engine.Dispose();
        _module.Dispose();
        _store.Dispose();
    }
}