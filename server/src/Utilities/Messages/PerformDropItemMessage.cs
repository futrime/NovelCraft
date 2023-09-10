using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientPerformDropItemMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformDropItem;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  public record ItemType {
    [JsonPropertyName("slot")]
    public required int Slot { get; init; }

    [JsonPropertyName("count")]
    public required int Count { get; init; }
  }

  [JsonPropertyName("drop_items")]
  public required List<ItemType> DropItems { get; init; }
}