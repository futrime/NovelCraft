using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{
    public record Velocity
    {
        [JsonProperty("x")]
        public decimal X { get; init; }

        [JsonProperty("y")]
        public decimal Y { get; init; }

        [JsonProperty("z")]
        public decimal Z { get; init; }
    }
}