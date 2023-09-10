using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformUseMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformUse;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        public enum UseKind
        {
            UseClick,
            // UseStart,
            // UseEnd
        }

        [JsonProperty("use_kind")]
        public UseKind UseType { get; init; }
    }
}