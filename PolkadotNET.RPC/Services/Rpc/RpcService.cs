using PolkadotNET.RPC.Namespaces;

namespace PolkadotNET.RPC.Services.Rpc;

class RpcService : BaseRpcService, IRpcService
{
    public RpcService(SmoldotJsonRpcClient rpcClient) : base(rpcClient)
    {
    }

    public Task<MethodsWrapper> Methods()
        => RpcClient.SendAsync<MethodsWrapper>("rpc_methods");
}