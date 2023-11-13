namespace PolkadotNET.RPC.Services.Rpc;

interface IRpcService
{
    public Task<MethodsWrapper> Methods();
}