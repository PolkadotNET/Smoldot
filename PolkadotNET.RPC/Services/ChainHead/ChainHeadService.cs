using PolkadotNET.RPC.Namespaces;
using PolkadotNET.RPC.Services.ChainHead.Parameters;
using PolkadotNET.RPC.Services.ChainHead.Results;
using PolkadotNET.RPC.Services.Rpc;
using ServiceStack;
using ServiceStack.Text;

namespace PolkadotNET.RPC.Services.ChainHead;

public class ChainHeadService : BaseRpcService, IChainHeadService
{
    private const string Namespace = "chainHead";
    private const string Version = "unstable";
    private const string Prefix = $"{Namespace}_{Version}";

    public delegate void Initialized(string followSubscription, Event.Initialized initialized);
    public delegate void NewBlock(string followSubscription, Event.NewBlock newBlock);
    public delegate void BestBlockChanged(string followSubscription, Event.BestBlockChanged bestBlockChanged);
    public delegate void Finalized(string followSubscription, Event.Finalized finalized);
    public delegate void OperationBodyDone(string followSubscription, Event.OperationBodyDone operationBodyDone);
    public delegate void OperationCallDone(string followSubscription, Event.OperationCallDone operationCallDone);
    public delegate void OperationStorageItems(string followSubscription, Event.OperationStorageItems operationStorageItems);
    public delegate void OperationWaitingForContinue(string followSubscription, Event.OperationWaitingForContinue operationWaitingForContinue);
    public delegate void OperationStorageDone(string followSubscription, Event.OperationStorageDone operationStorageDone);
    public delegate void OperationInaccessible(string followSubscription, Event.OperationInaccessible operationInaccessible);
    public delegate void OperationError(string followSubscription, Event.OperationError operationError);
    public delegate void Stop(string followSubscription, Event.Stop stop);

    public event Initialized? OnInitialized;
    public event NewBlock? OnNewBlock;
    public event BestBlockChanged? OnBestBlockChanged;
    public event Finalized? OnFinalized;
    public event OperationBodyDone? OnOperationBodyDone;
    public event OperationCallDone? OnOperationCallDone;
    public event OperationStorageItems? OnOperationStorageItems;
    public event OperationWaitingForContinue? OnOperationWaitingForContinue;
    public event OperationStorageDone? OnOperationStorageDone;
    public event OperationInaccessible? OnOperationInaccessible;
    public event OperationError? OnOperationError;
    public event Stop? OnStop;

    public ChainHeadService(SmoldotJsonRpcClient rpcClient) : base(rpcClient)
    {
        RpcClient.OnNotification += OnRpcNotification;
    }

    private void OnRpcNotification(SubscriptionMessage message)
    {
        if (message.Method != "chainHead_unstable_followEvent")
            return;
        
        // ToJson -> FromJson not so cool. How can be convert a JsonObject to T with consideration of DataContract properties?
        var receivedEvent = message.Params.Result.ToJson().FromJson<Event>();

        switch (receivedEvent.Result)
        {
            case Event.EventType.Initialized:
                OnInitialized?.Invoke(message.Params.Subscription, (Event.Initialized)receivedEvent);
                break;
            case Event.EventType.NewBlock:
                OnNewBlock?.Invoke(message.Params.Subscription, (Event.NewBlock)receivedEvent);
                break;
            case Event.EventType.BestBlockChanged:
                OnBestBlockChanged?.Invoke(message.Params.Subscription, (Event.BestBlockChanged)receivedEvent);
                break;
            case Event.EventType.Finalized:
                OnFinalized?.Invoke(message.Params.Subscription, (Event.Finalized)receivedEvent);
                break;
            case Event.EventType.OperationBodyDone:
                OnOperationBodyDone?.Invoke(message.Params.Subscription, (Event.OperationBodyDone)receivedEvent);
                break;
            case Event.EventType.OperationCallDone:
                OnOperationCallDone?.Invoke(message.Params.Subscription, (Event.OperationCallDone)receivedEvent);
                break;
            case Event.EventType.OperationStorageItems:
                OnOperationStorageItems?.Invoke(message.Params.Subscription, (Event.OperationStorageItems)receivedEvent);
                break;
            case Event.EventType.OperationWaitingForContinue:
                OnOperationWaitingForContinue?.Invoke(message.Params.Subscription, (Event.OperationWaitingForContinue)receivedEvent);
                break;
            case Event.EventType.OperationStorageDone:
                OnOperationStorageDone?.Invoke(message.Params.Subscription, (Event.OperationStorageDone)receivedEvent);
                break;
            case Event.EventType.OperationInaccessible:
                OnOperationInaccessible?.Invoke(message.Params.Subscription, (Event.OperationInaccessible)receivedEvent);
                break;
            case Event.EventType.OperationError:
                OnOperationError?.Invoke(message.Params.Subscription, (Event.OperationError)receivedEvent);
                break;
            case Event.EventType.Stop:
                OnStop?.Invoke(message.Params.Subscription, (Event.Stop)receivedEvent);
                break;
            default:
                throw new ArgumentOutOfRangeException(receivedEvent.Result.ToString());
        }
    }

    public Task<BodyResult> Body(BodyParameters parameters)
        => RpcClient.SendAsync<BodyParameters, BodyResult>($"{Prefix}_body", parameters);

    public Task<CallResult> Call(CallParameters parameters)
        => RpcClient.SendAsync<CallParameters, CallResult>($"{Prefix}_call", parameters);

    public Task Continue(ContinueParameters parameters)
        => RpcClient.SendAsync<ContinueParameters, object>($"{Prefix}_continue", parameters);

    public Task<string> FollowSubscription(FollowParameters parameters)
        => RpcClient.SendAsync<FollowParameters, string>($"{Prefix}_follow", parameters);

    public Task<string?> Header(HeaderParameters parameters)
        => RpcClient.SendAsync<HeaderParameters, string?>($"{Prefix}_header", parameters);

    public Task StopOperation(StopOperationParameters parameters)
        => RpcClient.SendAsync<StopOperationParameters, object>($"{Prefix}_stopOperation", parameters);

    public Task<StorageResult> Storage(StorageParameters parameters)
        => RpcClient.SendAsync<StorageParameters, StorageResult>($"{Prefix}_storage", parameters);

    public Task Unfollow(UnfollowOperationParameters parameters)
        => RpcClient.SendAsync<UnfollowOperationParameters, object>($"{Prefix}_unfollow", parameters);

    public Task Unpin(UnpinOperationParameters parameters)
        => RpcClient.SendAsync<UnpinOperationParameters, object>($"{Prefix}_unpin", parameters);
}