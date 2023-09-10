using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}

namespace NovelCraft.Utilities.Messages
{


    internal abstract record MessageBase : IMessage
    {
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

        [JsonProperty("bound_to")]
        public abstract IMessage.BoundToKind BoundTo { get; }

        [JsonProperty("type")]
        public abstract IMessage.MessageKind Type { get; }
    }
}