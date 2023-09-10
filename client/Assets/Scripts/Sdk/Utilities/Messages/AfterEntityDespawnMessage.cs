using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityDespawnMessage : MessageBase
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityDespawn;

        [JsonProperty("despawn_id_list")]
        public List<int> DespawnIdList { get; init; } = new();
    }
}