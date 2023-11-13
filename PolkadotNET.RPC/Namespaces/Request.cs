using System.Runtime.Serialization;

namespace PolkadotNET.RPC.Namespaces;

[DataContract]
public record Request<TParams>(string Method, string Id)
{
    [DataMember(Name = "jsonrpc")] public string JsonRpc => "2.0";

    [DataMember(Name = "params")] public TParams? Params { get; set; }

    [DataMember(Name = "method")] public string Method { get; set; } = Method;

    [DataMember(Name = "id")] public string Id { get; set; } = Id;
}