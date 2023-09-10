using Newtonsoft.Json;
using System.Collections.Generic;


namespace NovelCraft.Utilities.Messages
{



    internal record ClientGetTickMessage : MessageBase, IClientMessage
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetTick;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;
    }


    internal record ServerGetTickMessage : MessageBase
    {
        [JsonProperty("bound_to")]
        public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

        [JsonProperty("type")]
        public override IMessage.MessageKind Type => IMessage.MessageKind.GetTick;

        [JsonProperty("tick")]
        public int Tick { get; init; }

        [JsonProperty("ticks_per_second")]
        public decimal TicksPerSecond { get; init; }
    }
}