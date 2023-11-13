using PolkadotNET.RPC.Namespaces;

namespace PolkadotNET.RPC.Services;

public abstract class BaseRpcService
{
    protected readonly SmoldotJsonRpcClient RpcClient;

    protected BaseRpcService(SmoldotJsonRpcClient rpcClient)
    {
        RpcClient = rpcClient;
    }
}