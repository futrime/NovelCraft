using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformMergeSlotsMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformMergeSlots;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("from_slot")]
        public int FromSlot { get; init; }

        [JsonProperty("to_slot")]
        public int ToSlot { get; init; }
    }
}