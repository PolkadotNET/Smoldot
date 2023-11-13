using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.Rpc;

namespace PolkadotNET.RPC.Services.ChainSpec;

public class ChainSpecService: BaseRpcService, IChainSpecService
{
    public ChainSpecService(SmoldotJsonRpcClient rpcClient) : base(rpcClient)
    {
    }

    public Task<string> ChainName()
        => RpcClient.SendAsync<string>("chainSpec_v1_chainName");

    public Task<string> GenesisHash()
        => RpcClient.SendAsync<string>("chainSpec_v1_genesisHash");
}