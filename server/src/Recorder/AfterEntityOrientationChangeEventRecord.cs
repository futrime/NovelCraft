using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityOrientationChangeEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_orientation_change";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;


  public record DataType {
    [JsonPropertyName("change_list")]
    public required List<ChangeType> ChangeList { get; init; }
  }

  public record ChangeType {
    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("orientation")]
    public required OrientationType Orientation { get; init; }

    public record OrientationType {
      [JsonPropertyName("yaw")]
      public required decimal Yaw { get; init; }

      [JsonPropertyName("pitch")]
      public required decimal Pitch { get; init; }
    }
  }
}