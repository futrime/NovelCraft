using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityHurtMessage : MessageBase
    {
        public record HurtType
        {
            [JsonProperty("victim_unique_id")]
            public int VictimUniqueId { get; init; }

            [JsonProperty("damage")]
            public decimal Damage { get; init; }

            [JsonProperty("health")]
            public decimal? Health { get; init; }

            [JsonProperty("is_dead")]
            public bool? IsDead { get; init; }
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityHurt;

        [JsonProperty("hurt_list")]
        public List<HurtType> HurtList { get; init; } = new();
    }
}