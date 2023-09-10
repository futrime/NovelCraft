using Newtonsoft.Json;

namespace NovelCraft.Utilities.Messages
{


    public record Position<T> : IPosition<T>
    {
        [JsonProperty("x")]
        public T X { get; init; } = default!;

        [JsonProperty("y")]
        public T Y { get; init; } = default!;

        [JsonProperty("z")]
        public T Z { get; init; } = default!;
    }
}