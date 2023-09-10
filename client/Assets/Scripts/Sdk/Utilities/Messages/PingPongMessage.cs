using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{


    internal record ClientPingMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.Ping;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("sent_time")]
        public decimal SentTime { get; init; }
    }

    internal record ServerPongMessage : MessageBase
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.Ping;

        [JsonProperty("sent_time")]
        public decimal SentTime { get; init; }
    }
}