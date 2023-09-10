using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityHurtEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_hurt";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;


  public record DataType {
    [JsonPropertyName("hurt_list")]
    public required List<HurtType> HurtList { get; init; }
  }

  public record HurtType {
    public record DamageCauseType {
      [JsonPropertyName("kind")]
      public required int Kind { get; init; }

      [JsonPropertyName("attacker_unique_id")]
      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      public int? AttackerUniqueId { get; init; } = null;
    }

    [JsonPropertyName("victim_unique_id")]
    public required int VictimUniqueId { get; init; }

    [JsonPropertyName("damage")]
    public required decimal Damage { get; init; }

    [JsonPropertyName("health")]
    public required decimal Health { get; init; }

    [JsonPropertyName("is_dead")]
    public required bool IsDead { get; init; }

    [JsonPropertyName("damage_cause")]
    public required DamageCauseType DamageCause { get; init; }
  }
}
