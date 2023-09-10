using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterPlayerInventoryChangeMessage : MessageBase
    {
        public record ChangeType
        {
            [JsonProperty("player_unique_id")]
            public int PlayerUniqueId { get; init; }

            [JsonProperty("change_list")]
            public List<InventoryChangeType> ChangeList { get; init; } = new();

            public record InventoryChangeType
            {
                [JsonProperty("slot")]
                public int Slot { get; init; }

                [JsonProperty("count")]
                public int Count { get; init; }

                [JsonProperty("item_type_id")]
                [JsonIgnore]
                public int? ItemTypeId { get; init; }
            }
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterPlayerInventoryChange;

        [JsonProperty("change_list")]
        public List<ChangeType> ChangeList { get; init; } = new();
    }
}