using Wasmtime;

namespace wasm_test;

public static class WasiSnapshotPreviewOneLinkExtensions
{
    private const string ModuleName = "wasi_snapshot_preview1";

    public static void BindRandomGet(this Linker linker, CallerFunc<int, int, int> action)
        => linker.DefineFunction(ModuleName, "random_get", action);

    public static void BindClockTimeGet(this Linker linker, CallerFunc<int, long, int, int> action)
        => linker.DefineFunction(ModuleName, "clock_time_get", action);

    public static void BindFdWrite(this Linker linker, Func<int, int, int, int, int> action)
        => linker.DefineFunction(ModuleName, "fd_write", action);

    public static void BindSchedYield(this Linker linker, Func<int> action)
        => linker.DefineFunction(ModuleName, "sched_yield", action);

    public static void BindEnvironGet(this Linker linker, Func<int, int, int> action)
        => linker.DefineFunction(ModuleName, "environ_get", action);

    public static void BindEnvironSizesGet(this Linker linker, Func<int, int, int> action)
        => linker.DefineFunction(ModuleName, "environ_sizes_get", action);
    
    public static void BindProcExit(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "proc_exit", action);
}

public static class SmoldotLinkExtensions
{
    const string ModuleName = "smoldot";

    public static void BindPanic(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "panic", action);

    public static void BindCurrentTaskEntered(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "current_task_entered", action);

    public static void BindCurrentTaskExit(this Linker linker, Action action)
        => linker.DefineFunction(ModuleName, "current_task_exit", action);

    public static void BindConnectionTypeSupported(this Linker linker, Func<int, int> action)
        => linker.DefineFunction(ModuleName, "connection_type_supported", action);

    public static void BindJsonRpcResponseNonEmpty(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "json_rpc_responses_non_empty", action);

    public static void BindAdvanceExecutionReady(this Linker linker, Action action)
        => linker.DefineFunction(ModuleName, "advance_execution_ready", action);

    public static void BindBufferSize(this Linker linker, Func<int, int> action)
        => linker.DefineFunction(ModuleName, "buffer_size", action);

    public static void BindBufferCopy(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "buffer_copy", action);

    // float 64
    public static void BindStartTimer(this Linker linker, Action<double> action)
        => linker.DefineFunction(ModuleName, "start_timer", action);

    public static void BindLog(this Linker linker, CallerAction<int, int, int, int, int> action)
        => linker.DefineFunction(ModuleName, "log", action);

    public static void BindConnectionNew(this Linker linker, CallerAction<int, int, int> action)
        => linker.DefineFunction(ModuleName, "connection_new", action);

    public static void BindConnectionStreamOpen(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "connection_stream_open", action);

    public static void BindStreamSend(this Linker linker, Action<int, int, int, int> action)
        => linker.DefineFunction(ModuleName, "stream_send", action);

    public static void BindStreamSendClose(this Linker linker, Action<int, int> action)
        => linker.DefineFunction(ModuleName, "stream_send_close", action);

    public static void BindConnectionStreamReset(this Linker linker, Action<int, int> action)
        => linker.DefineFunction(ModuleName, "connection_stream_reset", action);

    public static void BindResetConnection(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "reset_connection", action);
}

public static class Bindings
{
    public delegate void Panic(uint messagePtr, uint messageLength);

    public delegate void BufferCopy(uint bufferIndex, uint targetPointer);

    public delegate uint BufferSize(uint bufferSize);

    public delegate void JsonRpcResponsesNonEmpty(uint chainId);

    public delegate void Log(uint level, uint targetPtr);

    public delegate void AdvanceExecutionReady();

    //f64?
    public delegate void StartTimer(float milliseconds);

    public delegate uint ConnectionTypeSupported(byte type);

    public delegate void ConnectionNew(uint id, uint addressPtr, uint addressLength);

    public delegate void ResetConnection(uint id);

    public delegate void ConnectionStreamOpen(uint connectionId);

    public delegate void ConnectionStreamReset(uint connectionId, uint streamId);

    public delegate void StreamSend(uint connectionId, uint streamId, uint ptr, uint len);

    public delegate void StreamSendClose(uint connectionId, uint streamId);

    public delegate void CurrentTaskEntered(uint ptr, uint len);

    public delegate void CurrentTaskExit();
}