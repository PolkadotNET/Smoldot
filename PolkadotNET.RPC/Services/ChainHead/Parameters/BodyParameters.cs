using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record BodyParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription,
    [property: DataMember(Name = "hash")] string Hash
);