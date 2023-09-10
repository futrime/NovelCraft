using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{

    internal record ServerAfterEntityRemoveMessage : MessageBase
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityRemove;

        [JsonProperty("removal_id_list")]
        public List<int> RemovalIdList { get; init; } = new();
    }
}