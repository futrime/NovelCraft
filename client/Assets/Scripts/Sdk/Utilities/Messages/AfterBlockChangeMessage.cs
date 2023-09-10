using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterBlockChangeMessage : MessageBase
    {
        public record ChangeType
        {
            [JsonProperty("position")]
            public Position<int> Position { get; init; } = new();

            [JsonProperty("block_type_id")]
            public int BlockTypeId { get; init; }
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterBlockChange;

        [JsonProperty("change_list")]
        public List<ChangeType> ChangeList { get; init; } = new();
    }
}