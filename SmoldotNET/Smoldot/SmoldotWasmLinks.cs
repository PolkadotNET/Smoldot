using System.Security.Cryptography;
using NLog;
using SmoldotNET.Connection;
using SmoldotNET.WasmMemory;
using SmoldotNET.WasmMemory.Structs;
using Wasmtime;

namespace SmoldotNET.Smoldot;

public class SmoldotWasmLinks : ISmoldotWasmLinks
{
    private readonly long _startupTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private readonly IMemoryTable _memoryTable;
    private readonly ITimerMap _timerMap;
    private readonly IConnectionManager _connectionManager;
    // ReSharper disable once NotAccessedField.Local
    private bool _shouldAdvance;
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    public SmoldotWasmLinks(IMemoryTable memoryTable, ITimerMap timerMap, ref bool shouldAdvance, IConnectionManager connectionManager)
    {
        _memoryTable = memoryTable;
        _timerMap = timerMap;
        _shouldAdvance = shouldAdvance;
        _connectionManager = connectionManager;
    }

    public void Link(Linker linker)
    {
        const string ModuleName = "smoldot";
        
        linker.DefineFunction<int, int>(ModuleName, "panic", Panic);
        linker.DefineFunction<int, int>(ModuleName, "random_get", RandomGet);
        linker.DefineFunction(ModuleName, "unix_timestamp_us", UnixTimestampUs);
        linker.DefineFunction(ModuleName, "monotonic_clock_us", MonotonicClockUs);
        linker.DefineFunction<int, int>(ModuleName, "buffer_copy", BufferCopy);
        linker.DefineFunction<int, int>(ModuleName, "buffer_size", BufferSize);
        linker.DefineFunction<int>(ModuleName, "json_rpc_responses_non_empty", JsonRpcResponseNonEmpty);
        linker.DefineFunction<int, int, int, int, int>(ModuleName, "log", Log);
        linker.DefineFunction(ModuleName, "advance_execution_ready", AdvanceExecutionReady);
        linker.DefineFunction<double>(ModuleName, "start_timer", StartTimer);
        linker.DefineFunction<int, int>(ModuleName, "connection_type_supported", ConnectionTypeSupported);
        linker.DefineFunction<int, int, int>(ModuleName, "connection_new", ConnectionNew);
        linker.DefineFunction<int>(ModuleName, "reset_connection", ResetConnection);
        linker.DefineFunction<int>(ModuleName, "connection_stream_open", ConnectionStreamOpen);
        linker.DefineFunction<int, int>(ModuleName, "connection_stream_reset", ConnectionStreamReset);
        linker.DefineFunction<int, int, int, int>(ModuleName, "stream_send", StreamSend);
        linker.DefineFunction<int, int>(ModuleName, "stream_send_close", StreamSendClose);
        linker.DefineFunction<int, int>(ModuleName, "current_task_entered", CurrentTaskEntered);
        linker.DefineFunction(ModuleName, "current_task_exit", CurrentTaskExit);
    }

    public long UnixTimestampUs(Caller caller)
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public long MonotonicClockUs(Caller caller)
        => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _startupTime;

    public void RandomGet(Caller caller, int targetPtr, int targetLen) =>
        caller.WriteByteArray(targetPtr, RandomNumberGenerator.GetBytes(targetLen));

    public void AdvanceExecutionReady() => _shouldAdvance = true;

    public void CurrentTaskEntered(Caller caller, int pointer, int length)
    {
    }

    public void CurrentTaskExit()
    {
    }

    public void ConnectionNew(Caller caller, int id, int addressPointer, int addressLength)
    {
        var newConnection = caller.Read<NewConnection>(addressPointer, addressLength);
        _connectionManager.CreateConnection(id, newConnection.AddressOrDomain, newConnection.Port);
    }

    public void ConnectionStreamOpen(int connectionId)
    {
        // currently only single-stream connections are supported, thus we dont make use of this.
    }

    public void ConnectionStreamReset(int connectionId, int streamId)
    {
        // currently only single-stream connections are supported, thus we dont make use of this.
    }

    private const int ConnectionTypeIsSupported = 1;
    private const int ConnectionTypeIsNotSupported = 0;

    public int ConnectionTypeSupported(int connectionType)
        => new ConnectionType((byte)connectionType).Protocol == Protocol.TcpIp
            ? ConnectionTypeIsSupported
            : ConnectionTypeIsNotSupported;

    public void JsonRpcResponseNonEmpty(int chainId)
    {
        // we dont care about this currently?
    }

    public void ResetConnection(int connectionId)
    {
        _logger.Warn($"Reset Connection ({connectionId})");
        _connectionManager.ResetConnection(connectionId);
    }

    public void StartTimer(double duration) => _timerMap.Add(duration);

    public void StreamSend(Caller caller, int connectionId, int streamId, int ptr, int len)
    {
        var memory = caller.GetMemory().GetSpan(ptr, len);
        _connectionManager.QueueOutbound(connectionId, memory.ToArray());
    }

    public void StreamSendClose(int connectionId, int streamId)
    {
        _logger.Warn("StreamSendClose?");
        // _connectionManager.ResetConnection(connectionId);
    }

    public int BufferSize(int index) => _memoryTable.GetBuffer(index).Length;

    public void BufferCopy(Caller caller, int bufferIndex, int targetPointer)
        => caller.WriteByteArray(targetPointer, _memoryTable.GetBuffer(bufferIndex));

    public void Panic(Caller caller, int messagePointer, int messageLength)
    {
        var error = caller.ReadStringFromMemory(messagePointer, messageLength);
        _logger.Error($"wasm paniced with '{error}'");
        throw new Exception($"wasm paniced with '{error}'");
    }

    public void Log(Caller caller, int level, int targetPointer, int targetLength, int messagePointer,
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
}