namespace SmoldotNET.Smoldot;

public interface ISmoldotWasmFunctions
{
    public void Init(int maxLogLevel);
    public void AdvanceExecution();
    public void TimerFinished();

    public int AddChain(
        int chainSpecBufferIndex,
        int dbContentBufferIndex,
        int jsonRpcMaxPendingRequests,
        int jsonRpcMaxSubscriptions,
        int relayChainsBufferIndex
    );

    public int ChainIsOk(int chainId);

    // JsonRpcInterface
    public int JsonRpcSend(int textBufferIndex, int chainId);
    public int JsonRpcResponsesPeek(int chainId);
    public void JsonRpcPop(int chainId);

    // Connection Manager
    public void ConnectionReset(int connectionId, int reasonBufferIndex);
    public void StreamWriteableBytes(int connectionId, int streamId, int additionalBytes);
    public void StreamMessage(int connectionId, int streamId, int messageBufferIndex);
}