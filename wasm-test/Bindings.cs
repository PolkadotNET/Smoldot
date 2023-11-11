using Wasmtime;

namespace wasm_test;

public static class SmoldotLinkExtensions
{
    /*
     *  CallerXXXX (e.g. CallerFunction / CallerAction) -> requiring access to caller e.g. for writing memory
     *  XXXX (e.g. Function / Action) -> do not require access to caller
     */
    
    const string ModuleName = "smoldot";

    public static void BindPanic(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "panic", action);
    
    public static void BindRandomGet(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "random_get", action);

    public static void BindUnixTimestampUs(this Linker linker, CallerFunc<long> action) =>
        linker.DefineFunction(ModuleName, "unix_timestamp_us", action);
    
    public static void BindMonotonicClockUs(this Linker linker, CallerFunc<long> action) =>
        linker.DefineFunction(ModuleName, "monotonic_clock_us", action);
    
    public static void BindBufferCopy(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "buffer_copy", action);
    
    public static void BindBufferSize(this Linker linker, Func<int, int> action)
        => linker.DefineFunction(ModuleName, "buffer_size", action);

    public static void BindJsonRpcResponseNonEmpty(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "json_rpc_responses_non_empty", action);
    
    public static void BindLog(this Linker linker, CallerAction<int, int, int, int, int> action)
        => linker.DefineFunction(ModuleName, "log", action);
    
    public static void BindAdvanceExecutionReady(this Linker linker, Action action)
        => linker.DefineFunction(ModuleName, "advance_execution_ready", action);
    
    public static void BindStartTimer(this Linker linker, Action<double> action)
        => linker.DefineFunction(ModuleName, "start_timer", action);
    
    public static void BindConnectionTypeSupported(this Linker linker, Func<int, int> action)
        => linker.DefineFunction(ModuleName, "connection_type_supported", action);

    public static void BindConnectionNew(this Linker linker, CallerAction<int, int, int> action)
        => linker.DefineFunction(ModuleName, "connection_new", action);
    
    public static void BindResetConnection(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "reset_connection", action);

    public static void BindConnectionStreamOpen(this Linker linker, Action<int> action)
        => linker.DefineFunction(ModuleName, "connection_stream_open", action);
    
    public static void BindConnectionStreamReset(this Linker linker, Action<int, int> action)
        => linker.DefineFunction(ModuleName, "connection_stream_reset", action);
    
    public static void BindStreamSend(this Linker linker, Action<int, int, int, int> action)
        => linker.DefineFunction(ModuleName, "stream_send", action);

    public static void BindStreamSendClose(this Linker linker, Action<int, int> action)
        => linker.DefineFunction(ModuleName, "stream_send_close", action);
    
    public static void BindCurrentTaskEntered(this Linker linker, CallerAction<int, int> action)
        => linker.DefineFunction(ModuleName, "current_task_entered", action);

    public static void BindCurrentTaskExit(this Linker linker, Action action)
        => linker.DefineFunction(ModuleName, "current_task_exit", action);
}