using Wasmtime;

namespace SmoldotNET.Smoldot;

public interface ISmoldotWasmLinks
{
    public void Link(Linker linker);
    public long UnixTimestampUs(Caller caller);
    public long MonotonicClockUs(Caller caller);
    public void RandomGet(Caller caller, int targetPtr, int targetLen);
    public void AdvanceExecutionReady();
    public void CurrentTaskEntered(Caller caller, int pointer, int length);
    public void CurrentTaskExit();
    public void ConnectionNew(Caller caller, int id, int addressPointer, int addressLength);
    public void ConnectionStreamOpen(int connectionId);
    public void ConnectionStreamReset(int connectionId, int streamId);
    public int ConnectionTypeSupported(int connectionType);
    public void JsonRpcResponseNonEmpty(int chainId);
    public void ResetConnection(int connectionId);
    public void StartTimer(double duration);
    public void StreamSend(Caller caller, int connectionId, int streamId, int ptr, int len);
    public void StreamSendClose(int connectionId, int streamId);
    public int BufferSize(int index);
    public void BufferCopy(Caller caller, int bufferIndex, int targetPointer);
    public void Panic(Caller caller, int messagePointer, int messageLength);

    public void Log(Caller caller, int level, int targetPointer, int targetLength, int messagePointer,
        int messageLength);
}