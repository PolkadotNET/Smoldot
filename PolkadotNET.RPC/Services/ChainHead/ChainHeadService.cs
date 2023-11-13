using System.Runtime.Serialization;
using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.Rpc;

namespace PolkadotNET.RPC.Services.ChainHead;

[DataContract]
public record FollowParameters([property: DataMember(Name = "withRuntime")]bool WithRuntime);

public class ChainHeadService : BaseRpcService, IChainHeadService
{
    public ChainHeadService(SmoldotJsonRpcClient rpcClient) : base(rpcClient)
    {
    }

    public Task<string> FollowSubscription(FollowParameters parameters)
        => RpcClient.SendAsync<FollowParameters, string>("chainHead_unstable_follow", parameters);

    public Task<string> Header(HeaderParameters parameters)
        => RpcClient.SendAsync<HeaderParameters, string>("chainHead_unstable_header", parameters);
}