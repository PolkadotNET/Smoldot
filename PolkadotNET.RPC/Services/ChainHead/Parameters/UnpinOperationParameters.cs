using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record UnpinOperationParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription,
    [property: DataMember(Name = "hashOrHashes")] string HashOrHashes
);