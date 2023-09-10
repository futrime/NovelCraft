using System.Text.Json.Serialization;

namespace NovelCraft.Server;

public record AgentInfo {
  [JsonPropertyName("name")]
  public required string Name { get; init; }

  [JsonIgnore]
  public int UniqueId { get; set; }
}
