namespace PolkadotNET.Smoldot.Smoldot;

public class Chain
{
    private readonly Smoldot _smoldot;
    private readonly int _id;

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


    public void Send(string json)
    {
        _smoldot.Send(_id, json);
    }

    private void OnRpcResponse(string response)
    {
        OnReceived?.Invoke(response);
    }

    public void Remove()
    {
    }
}