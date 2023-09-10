using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformLookAtMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformLookAt;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("look_at_position")]
        public Position<decimal> LookAtPosition { get; init; } = new();
    }
}