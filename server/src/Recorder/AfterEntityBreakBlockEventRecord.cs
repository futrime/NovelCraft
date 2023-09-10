using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityBreakBlockEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_break_block";

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

    [JsonPropertyName("block_type_id")]
    public required int BlockTypeId { get; init; }

    [JsonPropertyName("block_position")]
    public required PositionType BlockPosition { get; init; }
  }

  public record PositionType {
    [JsonPropertyName("x")]
    public required decimal X { get; init; }

    [JsonPropertyName("y")]
    public required decimal Y { get; init; }

    [JsonPropertyName("z")]
    public required decimal Z { get; init; }
  }
}
