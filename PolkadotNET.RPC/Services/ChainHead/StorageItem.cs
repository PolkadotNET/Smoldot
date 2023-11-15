using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead;

[DataContract]
public record StorageItem([property: DataMember(Name = "key")] string Key,
    [property: DataMember(Name = "value")] string Value, [property: DataMember(Name = "hash")] string Hash,
    [property: DataMember(Name = "closestDescendantMerkleValue")] string ClosestDescendantMerkleValue);