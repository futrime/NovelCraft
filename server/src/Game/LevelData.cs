using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

public record LevelData {
  [JsonPropertyName("type")]
  public string Type => "level_data";

  [JsonPropertyName("sections")]
  public required List<SectionType> Sections { get; init; }

  [JsonPropertyName("entities")]
  public required List<EntityType> Entities { get; init; }


  public record SectionType {
    [JsonPropertyName("x")]
    public required int X { get; init; }

    [JsonPropertyName("y")]
    public required int Y { get; init; }

    [JsonPropertyName("z")]
    public required int Z { get; init; }

    [JsonPropertyName("blocks")]
    public required List<int> Blocks { get; init; }
  }

  public record EntityType {
    [JsonPropertyName("entity_id")]
    public required int EntityId { get; init; }

    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("position")]
    public required PositionType Position { get; init; }

    [JsonPropertyName("orientation")]
    public required OrientationType Orientation { get; init; }


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
  }

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  public static LevelData NewFromJsonStr(string jsonString) {
    return JsonSerializer.Deserialize<LevelData>(jsonString)!;
  }
}