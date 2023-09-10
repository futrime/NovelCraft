using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientGetPlayerInfoMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetPlayerInfo;

  [JsonPropertyName("token")]
  public required string Token { get; init; }
}


public record ServerGetPlayerInfoMessage : MessageBase {
  public record ItemStackType {
    [JsonPropertyName("type_id")]
    public required int TypeId { get; init; }

    [JsonPropertyName("count")]
    public required int Count { get; init; }
  }

  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetPlayerInfo;

  [JsonPropertyName("health")]
  public required decimal Health { get; init; }

  [JsonPropertyName("orientation")]
  public required Orientation Orientation { get; init; }

  [JsonPropertyName("position")]
  public required Position<decimal> Position { get; init; }

  [JsonPropertyName("main_hand")]
  public required int MainHand { get; init; }

  [JsonPropertyName("inventory")]
  public required List<ItemStackType?> Inventory { get; init; }

  [JsonPropertyName("unique_id")]
  public required int UniqueId { get; init; }
}