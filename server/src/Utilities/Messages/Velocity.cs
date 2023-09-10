using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


public record Velocity {
  [JsonPropertyName("x")]
  public required decimal X { get; init; }

  [JsonPropertyName("y")]
  public required decimal Y { get; init; }

  [JsonPropertyName("z")]
  public required decimal Z { get; init; }
}