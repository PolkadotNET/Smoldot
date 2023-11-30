namespace PolkadotNET.Smoldot.Smoldot;

public class Chain : IDisposable
{
    private readonly Smoldot _smoldot;
    private readonly int _id;

    /// <summary>
    /// emits if the underlying smoldot instance has a json-rpc message (reply or notification) ready
    /// for this particular chain
    /// </summary>
    public event Received? OnReceived;

    public delegate void Received(string json);

    public Chain(Smoldot smoldot, int id)
    {
        _smoldot = smoldot;
        _id = id;

        _smoldot.OnRpcResponse += (chainId, response) =>
        {
            if (chainId == _id)
            {
                OnRpcResponse(response);
            }
        };
    }

    /// <summary>
    /// Submit a JSON RPC request to the underlying smoldot wasm instance
    /// </summary>
    /// <param name="json"></param>
    public void Send(string json)
    {
        _smoldot.Send(_id, json);
    }

    private void OnRpcResponse(string response)
    {
        OnReceived?.Invoke(response);
    }

    public void Dispose()
    {
        _smoldot.RemoveChain(_id);
    }
}