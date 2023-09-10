using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityPositionChangeMessage : MessageBase
    {
        public record ChangeType
        {
            [JsonProperty("unique_id")]
            public int UniqueId { get; init; }

            [JsonProperty("position")]
            public Position<decimal> Position { get; init; } = new();

            [JsonProperty("velocity")]
            public Velocity Velocity { get; init; } = new();
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityPositionChange;

        [JsonProperty("change_list")]
        public List<ChangeType> ChangeList { get; init; } = new();
    }
}