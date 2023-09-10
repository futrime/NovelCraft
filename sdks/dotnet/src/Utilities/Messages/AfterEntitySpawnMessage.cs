using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterEntitySpawnMessage : MessageBase {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntitySpawn;

  [JsonPropertyName("spawn_id_list")]
  public required List<int> SpawnIdList { get; init; }

}