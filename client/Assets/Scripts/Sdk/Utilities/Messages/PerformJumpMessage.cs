using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformJumpMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformJump;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;
    }
}