using PolkadotNET.RPC.Services.ChainHead.Parameters;
using PolkadotNET.RPC.Services.ChainHead.Results;

namespace PolkadotNET.RPC.Services.ChainHead;

interface IChainHeadService
{
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_body.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task<BodyResult> Body(BodyParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_call.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task<CallResult> Call(CallParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_continue.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Continue(ContinueParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_follow.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task<string> FollowSubscription(FollowParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_header.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task<string?> Header(HeaderParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_stopOperation.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task StopOperation(StopOperationParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_storage.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task<StorageResult> Storage(StorageParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_unfollow.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Unfollow(UnfollowOperationParameters parameters);
    
    /// <summary>
    /// https://paritytech.github.io/json-rpc-interface-spec/api/chainHead_unstable_unpin.html
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Task Unpin(UnpinOperationParameters parameters);
}