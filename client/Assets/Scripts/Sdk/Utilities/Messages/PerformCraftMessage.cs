using Newtonsoft.Json;
using System.Collections.Generic;

namespace NovelCraft.Utilities.Messages
{



    internal record ClientPerformCraftMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.PerformCraft;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonProperty("item_id_sequence")]
        public List<int?> ItemIdSequence { get; init; } = new();
    }
}