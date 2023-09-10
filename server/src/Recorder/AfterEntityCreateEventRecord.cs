using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityCreateEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_create";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }


  public record DataType {
    [JsonPropertyName("creation_list")]
    public required List<CreationType> CreationList { get; init; }
  }

  public record CreationType {
    [JsonPropertyName("entity_type_id")]
    public required int EntityTypeId { get; init; }

    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("position")]
    public required PositionType Position { get; init; }

    [JsonPropertyName("orientation")]
    public required OrientationType Orientation { get; init; }

    [JsonPropertyName("item_type_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ItemTypeId { get; init; } = null;

    [JsonPropertyName("health")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HealthType? Health { get; init; } = null;

    public record PositionType {
      [JsonPropertyName("x")]
      public required decimal X { get; init; }

      [JsonPropertyName("y")]
      public required decimal Y { get; init; }

      [JsonPropertyName("z")]
      public required decimal Z { get; init; }
    }

    public record OrientationType {
      [JsonPropertyName("yaw")]
      public required decimal Yaw { get; init; }

      [JsonPropertyName("pitch")]
      public required decimal Pitch { get; init; }
    }

    public record HealthType {
      [JsonPropertyName("health")]
      public required decimal Health { get; init; }

      [JsonPropertyName("is_dead")]
      public required bool IsDead { get; init; }
    }
  }
}