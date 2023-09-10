using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterPlayerSwitchMainHandEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_player_switch_main_hand";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  public record DataType {
    [JsonPropertyName("change_list")]
    public required List<ChangeType> ChangeList { get; init; }
  }

  public record ChangeType {
    [JsonPropertyName("player_unique_id")]
    public required int PlayerUniqueId { get; init; }
    [JsonPropertyName("new_main_hand")]
    public required int NewMainHand { get; init; }
  }
}