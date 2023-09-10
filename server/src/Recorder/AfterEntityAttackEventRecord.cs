using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterEntityAttackEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_entity_attack";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;


  public record DataType {
    [JsonPropertyName("attack_list")]
    public required List<AttackType> AttackList { get; init; }
  }

  public record AttackType {
    [JsonPropertyName("attacker_unique_id")]
    public required int AttackerUniqueId { get; init; }

    [JsonPropertyName("attack_kind")]
    public required string AttackKind { get; init; }
  }
}