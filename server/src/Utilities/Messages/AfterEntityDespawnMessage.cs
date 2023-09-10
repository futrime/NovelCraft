using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

public record ServerAfterEntityDespawnMessage : MessageBase {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityDespawn;

  [JsonPropertyName("despawn_id_list")]
  public required List<int> DespawnIdList { get; init; }
}