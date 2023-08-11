using System.Security.Cryptography;
using Wasmtime;

namespace wasm_test.WasmInstance;

public class WasmInstance : IDisposable
{
    private readonly Linker _linker;
    private readonly Engine _engine;
    private readonly Module _module;
    private readonly Store _store;

    private readonly Instance _instance;

    private readonly MemoryTable _memoryTable = new();

    private readonly Lazy<Action<int>> _init;
    private readonly Lazy<Action> _advanceExecution;

    public WasmInstance()
    {
        _engine = new Engine();
        _module = Module.FromFile(_engine, "./smoldot_light_wasm.wasm");
        _linker = new Linker(_engine);
        _store = new Store(_engine);

        BindWasmEvents();
        _instance = _linker.Instantiate(_store, _module);

        _init = new Lazy<Action<int>>(_instance.GetAction<int>("init")!);
        _advanceExecution = new Lazy<Action>(_instance.GetAction("advance_execution")!);
    }

    private void BindWasmEvents()
    {
        // Smoldot Bindings
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

        // WasiSnapshotPreview Bindings
        _linker.BindRandomGet(OnRandomGet);
        _linker.BindEnvironGet(OnEnvironGet);
        _linker.BindFdWrite(OnFdWrite);
        _linker.BindProcExit(OnProcExit);
        _linker.BindClockTimeGet(OnClockTimeGet);
        _linker.BindSchedYield(OnSchedYield);
        _linker.BindEnvironSizesGet(OnEnvironSizesGet);
    }

    private int OnRandomGet(Caller caller, int targetPtr, int targetLen)
    {
        caller.WriteByteArray(targetPtr, RandomNumberGenerator.GetBytes(targetLen));
        return 0;
    }

    private int OnClockTimeGet(Caller caller, int clockId, long precision, int outPtr)
    {
        Console.WriteLine("OnClockTimeGet");
        // TODO!
        return 20;
    }

    private int OnFdWrite(int fd, int addr, int num, int outPtr)
    {
        Console.WriteLine("OnFdWrite");
        return 8;
    }

    private void OnProcExit(int returnCode)
    {
        throw new Exception($"WASM ProcExit: {returnCode}");
    }

    private int OnEnvironSizesGet(int argcOut, int argvBufSizeOut)
    {
        Console.WriteLine($"OnEnvironSizesGet");
        return 0;
    }

    private int OnEnvironGet(int argv, int argvBuf)
    {
        Console.WriteLine("OnEnvironGet");
        return 0;
    }

    private int OnSchedYield()
    {
        Console.WriteLine("OnSchedYield");
        return 0;
    }

    private void OnAdvanceExecutionReady()
    {
        Console.WriteLine("OnAdvanceExecutionReady");
    }

    private void OnCurrentTaskEntered(Caller caller, int pointer, int length)
    {
        var task = caller.ReadStringFromMemory(pointer, length);
        Console.WriteLine($"Task Entered: {task}");
    }

    private void OnCurrentTaskExit()
    {
        Console.WriteLine("Current Task Exited");
    }

    private void OnConnectionNew(Caller caller, int id, int addressPointer, int addressLength)
    {
        Console.WriteLine($"OnConnectionNew");
    }

    private void OnConnectionStreamOpen(int connectionId)
    {
        Console.WriteLine($"Connection Stream {connectionId} opened");
    }

    private void OnConnectionStreamReset(int connectionId, int streamId)
    {
        Console.WriteLine($"Connection {connectionId} Stream {streamId} reset");
    }

    private int OnConnectionTypeSupported(int connectionType)
    {
        Console.WriteLine($"OnConnectionTypeSupported {connectionType}");
        return 0;
    }

    private void OnJsonRpcResponseNonEmpty(int chainId)
    {
        Console.WriteLine("OnJsonRpcResponseNonEmpty");
    }

    private void OnResetConnection(int connectionId)
    {
        Console.WriteLine($"OnConnectionReset: {connectionId}");
    }

    private void OnStartTimer(double duration)
    {
        Console.WriteLine($"OnStartTimer: {duration}");
    }

    private void OnStreamSend(int connectionId, int streamId, int ptr, int len)
    {
        Console.WriteLine($"OnStreamSend");
    }

    private void OnStreamSendClose(int connectionId, int streamId)
    {
        Console.Write($"OnStreamSendClose");
    }

    private int OnBufferSize(int index) => _memoryTable.GetBuffer(index).Length;

    private void OnBufferCopy(Caller caller, int bufferIndex, int targetPointer)
    {
        Console.WriteLine($"Copying Buffer {bufferIndex} into Memory at {targetPointer}");
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

        Console.WriteLine($"Log ({target}): {message}");
    }

    public void Init(int maxLogLevel) => _init.Value(maxLogLevel);
    public void AdvanceExecution() => _advanceExecution.Value();

    public void Dispose()
    {
        _linker.Dispose();
        _engine.Dispose();
        _module.Dispose();
        _store.Dispose();
    }
}