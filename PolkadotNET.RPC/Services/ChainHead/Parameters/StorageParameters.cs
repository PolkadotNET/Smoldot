using System.Runtime.Serialization;
using PolkadotNET.RPC.Services.ChainHead.Results;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record StorageParameters(
    [property: DataMember(Name = "followSubscription")]
    string FollowSubscription,
    [property: DataMember(Name = "hash")] string Hash,
    [property: DataMember(Name = "items")] Item[] Items,
    [property: DataMember(Name = "childTrie")]
    string? ChildTrie
);