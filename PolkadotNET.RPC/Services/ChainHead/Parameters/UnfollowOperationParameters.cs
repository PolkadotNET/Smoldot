using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record UnfollowOperationParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription
);