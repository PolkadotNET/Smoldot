namespace PolkadotNET.RPC.Namespaces;

public class JsonRpcException : Exception
{
    public JsonRpcException(int code, string message) : base(message)
    {
        Code = code;
    }

    public int Code { get; set; }

    public override string ToString()
        => $"Code: {Code}, Message: {Message}";
}