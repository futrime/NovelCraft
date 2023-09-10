using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{


    internal record ClientPerformRotateMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformRotate;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("orientation")]
        public Orientation Orientation { get; init; } = new();
    }
}