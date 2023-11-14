using Wasmtime;

namespace SmoldotNET.Smoldot;

public class SmoldotWasmFunctions : ISmoldotWasmFunctions
{
    private readonly Instance _instance;

    public SmoldotWasmFunctions(Instance instance)
    {
        _instance = instance;
    }

    public void Init(int maxLogLevel) =>
        _instance.GetAction<int>("init")!.Invoke(maxLogLevel);

    public void AdvanceExecution() =>
        _instance.GetAction("advance_execution")!.Invoke();

    public void TimerFinished() =>
        _instance.GetAction("timer_finished")!.Invoke();

    public int AddChain(
        int chainSpecBufferIndex,
        int dbContentBufferIndex,
        int jsonRpcMaxPendingRequests,
        int jsonRpcMaxSubscriptions,
        int relayChainsBufferIndex) =>
        _instance.GetFunction<
            int, int, int,
            int, int, int
        >("add_chain")!.Invoke(
            chainSpecBufferIndex,
            dbContentBufferIndex,
            jsonRpcMaxPendingRequests,
            jsonRpcMaxSubscriptions,
            relayChainsBufferIndex
        );

    public int ChainIsOk(int chainId) =>
        _instance.GetFunction<int, int>("chain_is_ok")!.Invoke(chainId);

    public int JsonRpcSend(int textBufferIndex, int chainId) =>
        _instance.GetFunction<int, int, int>("json_rpc_send")!.Invoke(textBufferIndex, chainId);

    public int JsonRpcResponsesPeek(int chainId) =>
        _instance.GetFunction<int, int>("json_rpc_responses_peek")!.Invoke(chainId);

    public void JsonRpcPop(int chainId) =>
        _instance.GetAction<int>("json_rpc_responses_pop")!.Invoke(chainId);

    public void ConnectionReset(int connectionId, int reasonBufferIndex) =>
        _instance.GetAction<int, int>("connection_reset")!.Invoke(connectionId, reasonBufferIndex);

    public void StreamWriteableBytes(
        int connectionId,
        int streamId,
        int additionalBytes
    )  {
        _instance.GetAction<
            int, int, int
        >("stream_writable_bytes")!.Invoke(
            connectionId,
            streamId,
            additionalBytes
        );
    }

    public void StreamMessage(
        int connectionId,
        int streamId,
        int messageBufferIndex
    ) =>
        _instance.GetAction<
            int, int, int
        >("stream_message")!.Invoke(
            connectionId,
            streamId,
            messageBufferIndex
        );
}