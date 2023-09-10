using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityCreateMessage : MessageBase
    {
        public record CreationType
        {
            [JsonProperty("entity_type_id")]
            public int EntityTypeId { get; init; }

            [JsonProperty("unique_id")]
            public int UniqueId { get; init; }

            [JsonProperty("position")]
            public Position<decimal> Position { get; init; } = new();

            [JsonProperty("orientation")]
            public Orientation Orientation { get; init; } = new();

            [JsonProperty("item_type_id")]
            public int? ItemTypeId { get; init; } = null;

            [JsonProperty("health")]
            public decimal? Health { get; init; } = null;
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityCreate;

        [JsonProperty("creation_list")]
        public List<CreationType> CreationList { get; init; } = new();
    }
}