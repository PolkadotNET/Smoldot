using ServiceStack;
using ServiceStack.Text;
using SmoldotNET.Smoldot;

namespace PolkadotNET.RPC.Namespaces;

public class SmoldotJsonRpcClient
{
    private readonly Chain _chain;
    private int _messageCounter;

    private readonly Dictionary<int, ManualResetEventSlim> _mutexTable = new();
    private readonly Dictionary<int, string> _responseTable = new();

    public delegate void Notification(SubscriptionMessage subscriptionMessage);

    public event Notification? OnNotification;

    public SmoldotJsonRpcClient(Chain chain)
    {
        _messageCounter = 1;
        _chain = chain;

        _chain.OnReceived += OnChainRpcReceived;
    }

    private void OnChainRpcReceived(string json)
    {
        var response = json.FromJson<JsonObject>();

        var id = response["id"];
        if (string.IsNullOrEmpty(id))
        {
            // notification
            OnNotification?.Invoke(json.FromJson<SubscriptionMessage>());
        }
        else
        {
            // response
            _responseTable.Add(int.Parse(id), json);
            _mutexTable[int.Parse(id)].Set();
        }
    }

    public Task<TResponse> SendAsync<TResponse>(string method)
    {
        var id = _messageCounter++;
        return SendAsync<TResponse>(id, new Request<object>(method, id.ToString()).ToJson());
    }

    private Task<TResponse> SendAsync<TResponse>(int id, string request)
    {
        return Task.Run(() =>
        {
            var resetHandle = new ManualResetEventSlim();
            _mutexTable.Add(id, resetHandle);
            _chain.Send(request);

            resetHandle.Wait();

            var jsonResponse = _responseTable[id];

            _responseTable.Remove(id);
            _mutexTable.Remove(id);

            var responsePeek = jsonResponse.FromJson<JsonObject>();
            if (!string.IsNullOrEmpty(responsePeek["error"]))
            {
                var errorResponse = jsonResponse.FromJson<ErrorResponse>();
                throw new JsonRpcException(errorResponse.Error.Code, errorResponse.Error.Message);
            }

            return jsonResponse.FromJson<SuccessResponse<TResponse>>().Result;
        });
    }

    public Task<TResponse> SendAsync<TRequest, TResponse>(string method, TRequest payload)
    {
        var id = _messageCounter++;
        return SendAsync<TResponse>(id, new Request<TRequest>(method, id.ToString())
        {
            Params = payload
        }.ToJson());
    }
}