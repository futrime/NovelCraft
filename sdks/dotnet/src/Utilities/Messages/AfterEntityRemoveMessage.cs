using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterEntityRemoveMessage : MessageBase {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityRemove;

  [JsonPropertyName("removal_id_list")]
  public required List<int> RemovalIdList { get; init; }
}