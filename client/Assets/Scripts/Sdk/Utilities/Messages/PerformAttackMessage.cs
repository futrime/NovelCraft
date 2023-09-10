using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformAttackMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformAttack;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        public enum AttackType
        {
            AttackClick,
            HoldStart,
            HoldEnd
        };

        [JsonProperty("attack_kind")]
        public AttackType AttackKind { get; init; }


    }
}