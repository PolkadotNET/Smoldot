using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Namespaces;

[DataContract]
public record SubscriptionMessage(
    [property: DataMember(Name = "jsonrpc")]
    string JsonRpc,
    [property: DataMember(Name = "method")]
    string Method,
    [property: DataMember(Name = "params")]
    Params Params
);