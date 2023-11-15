using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record CallParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription,
    [property: DataMember(Name = "hash")] string Hash,
    [property: DataMember(Name = "function")]
    string Function,
    [property: DataMember(Name = "callParameters")]
    string Parameters
);