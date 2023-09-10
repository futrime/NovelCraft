using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NovelCraft.Utilities.Messages
{

    internal record EmptyMessage : IMessage
    {
        [JsonProperty("bound_to")]
        public IMessage.BoundToKind BoundTo { get; init; }

        [JsonProperty("type")]
        public IMessage.MessageKind Type { get; init; }

        [JsonIgnore]
        public JToken Json
        {
            get => JToken.Parse(JsonConvert.SerializeObject((object)this))!;
        }

        [JsonIgnore]
        public string JsonString
        {
            get => JsonConvert.SerializeObject((object)this);
        }
    }
}