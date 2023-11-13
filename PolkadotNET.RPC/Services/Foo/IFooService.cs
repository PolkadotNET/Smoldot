using PolkadotNET.RPC.Namespaces;

namespace PolkadotNET.RPC.Services.Foo;

interface IFooService
{
    public Task<string> Bar();
}

public class FooService : BaseRpcService, IFooService
{
    public FooService(SmoldotJsonRpcClient rpcClient) : base(rpcClient)
    {
    }

    public Task<string> Bar()
        => RpcClient.SendAsync<string>("chain_subscribeNewHead");
}