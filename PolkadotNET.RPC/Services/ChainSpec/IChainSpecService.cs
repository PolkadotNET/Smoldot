namespace PolkadotNET.RPC.Services.ChainSpec;

public interface IChainSpecService
{
    public Task<string> ChainName();
    public Task<string> GenesisHash();
}