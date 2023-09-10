using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformSwapSlotsMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformSwapSlots;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("slot_a")]
        public int SlotA { get; init; }

        [JsonProperty("slot_b")]
        public int SlotB { get; init; }
    }
}