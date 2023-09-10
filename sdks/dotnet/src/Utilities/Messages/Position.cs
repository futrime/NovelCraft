using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


public record Position<T> {
  [JsonPropertyName("x")]
  public required T X { get; init; }

  [JsonPropertyName("y")]
  public required T Y { get; init; }

  [JsonPropertyName("z")]
  public required T Z { get; init; }
}