using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Namespaces;

[DataContract]
public record SuccessResponse<TResult>(
    [property: DataMember(Name = "jsonrpc")]
    string JsonRpc,
    [property: DataMember(Name = "id")] string Id,
    [property: DataMember(Name = "result")]
    TResult Result
);