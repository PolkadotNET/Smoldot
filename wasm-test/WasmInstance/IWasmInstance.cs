namespace wasm_test.WasmInstance;

public interface IWasmInstance
{
    public void Init(int maxLogLevel);
    public void AdvanceExecution();
    public int AddChain(int chainSpecBufferIndex, int databaseContentBufferIndex, int jsonRpcMaxPendingRequests,
        int jsonRpcMaxSubscriptions, int potentialRelayChainsBufferIndex);
    public void RemoveChain(int chainId);
    public int ChainIsOk(int chainId);
    public int ChainErrorLen(int chainId);
    public int ChainErrorPtr(int chainId);
    public int JsonRpcSend(int textBufferIndex, int chainId);
    public int JsonRpcResponsesPeek(int chainId);
}