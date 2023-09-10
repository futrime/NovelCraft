using Newtonsoft.Json;
using System.Collections.Generic;


namespace NovelCraft.Utilities.Messages
{


    internal record ErrorMessage : MessageBase, IErrorMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.Error;

        [JsonProperty("code")]
        public int Code { get; init; }

        [JsonProperty("message")]
        public string Message { get; init; } = string.Empty;
    }
}