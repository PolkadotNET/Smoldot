using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Namespaces;

[DataContract]
public record ErrorResponse(
    [property: DataMember(Name = "jsonrpc")]
    string JsonRpc,
    [property: DataMember(Name = "id")] string Id,
    [property: DataMember(Name = "error")] Error Error
);