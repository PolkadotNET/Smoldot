using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Services.ChainHead.Parameters;

[DataContract]
public record FollowParameters([property: DataMember(Name = "withRuntime")]bool WithRuntime);