using Newtonsoft.Json;
using System.Collections.Generic;


namespace NovelCraft.Utilities.Messages
{



    internal record ClientGetPlayerInfoMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetPlayerInfo;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;
    }


    internal record ServerGetPlayerInfoMessage : MessageBase
    {
        public record ItemStackType
        {
            [JsonProperty("type_id")]
            public int TypeId { get; init; }

            [JsonProperty("count")]
            public int Count { get; init; }
        }

        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetPlayerInfo;

        [JsonProperty("health")]
        public decimal Health { get; init; }

        [JsonProperty("orientation")]
        public Orientation Orientation { get; init; } = new();

        [JsonProperty("position")]
        public Position<decimal> Position { get; init; } = new();

        [JsonProperty("main_hand")]
        public int MainHand { get; init; }

        [JsonProperty("inventory")]
        public List<ItemStackType?> Inventory { get; init; } = new();

        [JsonProperty("unique_id")]
        public int UniqueId { get; init; }
    }
}