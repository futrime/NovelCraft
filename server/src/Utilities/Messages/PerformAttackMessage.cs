using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientPerformAttackMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformAttack;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  public enum AttackType {
    AttackClick,
    HoldStart,
    HoldEnd
  };

  [JsonPropertyName("attack_kind")]
  public required AttackType AttackKind { get; init; }


}