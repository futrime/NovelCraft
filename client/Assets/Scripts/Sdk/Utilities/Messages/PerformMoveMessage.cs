using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{
    internal record ClientPerformMoveMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformMove;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;
        public enum Direction
        {
            Stop,
            Forward,
            Backward,
            Left,
            Right
        }
        [JsonProperty("direction")]
        public Direction DirectionType { get; init; }
    }
}