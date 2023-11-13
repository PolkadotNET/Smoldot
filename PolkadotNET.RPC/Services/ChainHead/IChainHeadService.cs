using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead;

[DataContract]
public record HeaderParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription,
    [property: DataMember(Name = "hash")] string Hash
);

interface IChainHeadService
{
    public Task<string> FollowSubscription(FollowParameters parameters);
    public Task<string> Header(HeaderParameters parameters);
}