using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{
    internal record ServerAfterEntityAttackMessage : MessageBase
    {
        public record AttackType
        {
            public enum AttackKind { Click, HoldStart, HoldEnd }


            [JsonProperty("attacker_unique_id")]
            public int AttackerUniqueId { get; init; }

            [JsonProperty("kind")]
            public AttackKind Kind { get; init; }
        }


        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityAttack;

        [JsonProperty("attack_list")]
        public List<AttackType> AttackList { get; init; } = new();
    }
}