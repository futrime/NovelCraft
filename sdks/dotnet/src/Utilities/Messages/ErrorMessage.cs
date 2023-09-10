using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


internal record ErrorMessage : MessageBase, IErrorMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.Error;

  [JsonPropertyName("code")]
  public required int Code { get; init; }

  [JsonPropertyName("message")]
  public required string Message { get; init; }
}