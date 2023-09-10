using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformDropItemMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformDropItem;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        public record ItemType
        {
            [JsonProperty("slot")]
            public int Slot { get; init; }

            [JsonProperty("count")]
            public int Count { get; init; }
        }

        [JsonProperty("drop_items")]
        public List<ItemType> DropItems { get; init; } = new();
    }
}