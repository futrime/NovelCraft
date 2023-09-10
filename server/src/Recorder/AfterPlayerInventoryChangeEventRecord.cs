using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterPlayerInventoryChangeEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_player_inventory_change";

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
    [JsonPropertyName("player_unique_id")]
    public required int PlayerUniqueId { get; init; }

    [JsonPropertyName("change_list")]
    public required List<InventoryChangeType> ChangeList { get; init; }


    public record InventoryChangeType {
      [JsonPropertyName("slot")]
      public required int Slot { get; init; }

      [JsonPropertyName("count")]
      public required int Count { get; init; }

      [JsonPropertyName("item_type_id")]
      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      public int? ItemTypeId { get; init; }
    }
  }
}