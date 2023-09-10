using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityHealEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_heal";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;


  public record DataType {
    [JsonPropertyName("entity_unique_id")]
    public required int EntityUniqueId { get; init; }

    [JsonPropertyName("heal_amount")]
    public required decimal HealAmount { get; init; }

    [JsonPropertyName("health")]
    public required decimal Health { get; init; }
  }
}
