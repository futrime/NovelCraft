using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

public record ServerAfterEntityHurtMessage : MessageBase {
  public record HurtType {
    [JsonPropertyName("victim_unique_id")]
    public required int VictimUniqueId { get; init; }

    [JsonPropertyName("damage")]
    public required decimal Damage { get; init; }

    [JsonPropertyName("health")]
    public required decimal? Health { get; init; }

    [JsonPropertyName("is_dead")]
    public required bool? IsDead { get; init; }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityHurt;

  [JsonPropertyName("hurt_list")]
  public required List<HurtType> HurtList { get; init; }
}