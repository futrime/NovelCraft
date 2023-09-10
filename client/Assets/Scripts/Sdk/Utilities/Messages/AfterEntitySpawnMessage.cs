using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntitySpawnMessage : MessageBase
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntitySpawn;

        [JsonProperty("spawn_id_list")]
        public List<int> SpawnIdList { get; init; } = new();

    }
}