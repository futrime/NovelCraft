using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterBlockChangeEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_block_change";

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
    [JsonPropertyName("position")]
    public required PositionType Position { get; init; }

    [JsonPropertyName("block_type_id")]
    public required int BlockTypeId { get; init; }


    public record PositionType {
      [JsonPropertyName("x")]
      public required int X { get; init; }

      [JsonPropertyName("y")]
      public required int Y { get; init; }

      [JsonPropertyName("z")]
      public required int Z { get; init; }
    }
  }
}