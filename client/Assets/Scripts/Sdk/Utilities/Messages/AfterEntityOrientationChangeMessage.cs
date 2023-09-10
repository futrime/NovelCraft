using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityOrientationChangeMessage : MessageBase
    {
        public record ChangeType
        {
            [JsonProperty("unique_id")]
            public int UniqueId { get; init; }

            [JsonProperty("orientation")]
            public Orientation Orientation { get; init; } = new();
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityOrientationChange;

        [JsonProperty("change_list")]
        public List<ChangeType> ChangeList { get; init; } = new();
    }
}