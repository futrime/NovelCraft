using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityDespawnEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_despawn";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  public record DataType {
    [JsonPropertyName("despawn_list")]
    public required List<DespawnType> DespawnList { get; init; }
  }

  public record DespawnType {
    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }
  }
}