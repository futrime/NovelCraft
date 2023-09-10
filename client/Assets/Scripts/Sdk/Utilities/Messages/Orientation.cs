using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{
    public record Orientation : IOrientation
    {
        [JsonProperty("yaw")]
        public decimal Yaw { get; init; }

        [JsonProperty("pitch")]
        public decimal Pitch { get; init; }
    }
}