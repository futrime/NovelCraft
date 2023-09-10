using Newtonsoft.Json;
using System.Collections.Generic;


namespace NovelCraft.Utilities.Messages
{



    internal record ClientGetBlocksAndEntitiesMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetBlocksAndEntities;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("request_section_list")]
        public List<Position<int>> RequestSectionList { get; init; } = new();
    }


    internal record ServerGetBlocksAndEntitiesMessage : MessageBase
    {
        public record SectionType
        {
            [JsonProperty("position")]
            public Position<int> Position { get; init; } = new();

            [JsonProperty("blocks")]
            public List<int> Blocks { get; init; } = new();
        }

        public record EntityType
        {
            [JsonProperty("type_id")]
            public int TypeId { get; init; }

            [JsonProperty("unique_id")]
            public int UniqueId { get; init; }

            [JsonProperty("position")]
            public Position<decimal> Position { get; init; } = new();

            [JsonProperty("orientation")]
            public Orientation Orientation { get; init; } = new();
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetBlocksAndEntities;

        [JsonProperty("sections")]
        public List<SectionType> Sections { get; init; } = new();

        [JsonProperty("entities")]
        public List<EntityType> Entities { get; init; } = new();
    }
}