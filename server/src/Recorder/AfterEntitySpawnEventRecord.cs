using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntitySpawnEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_spawn";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  public record DataType {
    [JsonPropertyName("spawn_list")]
    public required List<SpawnType> SpawnList { get; init; }
  }

  public record SpawnType {
    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }
  }
}