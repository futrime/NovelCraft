using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformSwitchMainHandSlotMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformSwitchMainHandSlot;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("new_main_hand")]
        public int NewMainHand { get; init; }
    }
}