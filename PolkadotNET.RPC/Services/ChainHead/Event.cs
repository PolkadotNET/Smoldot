using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.Text;

namespace PolkadotNET.RPC.Services.ChainHead;

public abstract record Event
{
    public static Event ParseJson(string json)
    {
        var jsonObject = json.FromJson<JsonObject>();
        return jsonObject["event"] switch
        {
            "initialized" => json.FromJson<Initialized>(),
            "newBlock" => json.FromJson<NewBlock>(),
            "bestBlockChanged" => json.FromJson<BestBlockChanged>(),
            "finalized" => json.FromJson<Finalized>(),
            "operationBodyDone" => json.FromJson<OperationBodyDone>(),
            "operationCallDone" => json.FromJson<OperationCallDone>(),
            "operationStorageItems" => json.FromJson<OperationStorageItems>(),
            "operationWaitingForContinue" => json.FromJson<OperationWaitingForContinue>(),
            "operationStorageDone" => json.FromJson<OperationStorageDone>(),
            "operationInaccessible" => json.FromJson<OperationInaccessible>(),
            "operationError" => json.FromJson<OperationError>(),
            "stop" => json.FromJson<Stop>(),
            _ => throw new ArgumentOutOfRangeException(jsonObject["event"])
        };
    }

    public enum EventType
    {
        Initialized,
        NewBlock,
        BestBlockChanged,
        Finalized,
        OperationBodyDone,
        OperationCallDone,
        OperationStorageItems,
        OperationWaitingForContinue,
        OperationStorageDone,
        OperationInaccessible,
        OperationError,
        Stop
    }

    public abstract EventType Result { get; }

    [DataContract]
    public record Initialized(
        [property: DataMember(Name = "finalizedBlockHash")]
        string FinalizedBlockHash,
        [property: DataMember(Name = "finalizedBlockRuntime")]
        object? FinalizedBlockRuntime
    ) : Event
    {
        public override EventType Result => EventType.Initialized;
    }

    [DataContract]
    public record NewBlock(
        [property: DataMember(Name = "blockHash")]
        string BlockHash,
        [property: DataMember(Name = "parentBlockHash")]
        string ParentBlockHash,
        [property: DataMember(Name = "newRuntime")]
        object? NewRuntime
    ) : Event
    {
        public override EventType Result => EventType.NewBlock;
    }

    [DataContract]
    public record BestBlockChanged(
        [property: DataMember(Name = "bestBlockHash")]
        string BestBlockHash
    ) : Event
    {
        public override EventType Result => EventType.BestBlockChanged;
    }

    [DataContract]
    public record Finalized(
        [property: DataMember(Name = "finalizedBlockHashes")]
        string[] FinalizedBlockHashes,
        [property: DataMember(Name = "prunedBlockHashes")]
        string[] PrunedBlockHashes
    ) : Event
    {
        public override EventType Result => EventType.Finalized;
    }

    [DataContract]
    public record OperationBodyDone(
        [property: DataMember(Name = "operationId")]
        string OperationId,
        [property: DataMember(Name = "value")] string[] Value
    ) : Event
    {
        public override EventType Result => EventType.OperationBodyDone;
    }

    [DataContract]
    public record OperationCallDone(
        [property: DataMember(Name = "operationId")]
        string OperationId,
        [property: DataMember(Name = "output")]
        string Output
    ) : Event
    {
        public override EventType Result => EventType.OperationCallDone;
    }

    [DataContract]
    public record OperationStorageItems(
        [property: DataMember(Name = "operationId")]
        string OperationId,
        [property: DataMember(Name = "items")] StorageItem[] Items
    ) : Event
    {
        public override EventType Result => EventType.OperationStorageItems;
    }

    [DataContract]
    public record OperationWaitingForContinue(
        [property: DataMember(Name = "operationId")]
        string OperationId
    ) : Event
    {
        public override EventType Result => EventType.OperationWaitingForContinue;
    }

    [DataContract]
    public record OperationStorageDone(
        [property: DataMember(Name = "operationId")]
        string OperationId
    ) : Event
    {
        public override EventType Result => EventType.OperationStorageDone;
    }

    [DataContract]
    public record OperationInaccessible(
        [property: DataMember(Name = "operationId")]
        string OperationId
    ) : Event
    {
        public override EventType Result => EventType.OperationInaccessible;
    }

    [DataContract]
    public record OperationError(
        [property: DataMember(Name = "operationId")]
        string OperationId,
        [property: DataMember(Name = "error")] string Error
    ) : Event
    {
        public override EventType Result => EventType.OperationError;
    }

    [DataContract]
    public record Stop : Event
    {
        public override EventType Result => EventType.Stop;
    }
}