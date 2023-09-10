using NovelCraft.Utilities.Messages;

namespace NovelCraft.Server;

public partial class Binder {
  private void SubscribeToServer() {
    _server.AfterReceiveMessageEvent += (sender, e) => {
      try {
        switch (e.Message) {
          case ClientGetBlocksAndEntitiesMessage message:
            OnServerAfterReceiveGetBlocksAndEntitiesMessage(e.UniqueId, message);
            break;

          case ClientGetPlayerInfoMessage message:
            OnServerAfterReceiveGetPlayerInfoMessage(e.UniqueId, message);
            break;

          case ClientGetTickMessage message:
            OnServerAfterReceiveGetTickMessage(e.UniqueId, message);
            break;

          case ClientPerformAttackMessage message:
            OnServerAfterReceivePerformAttackMessage(e.UniqueId, message);
            break;

          case ClientPerformCraftMessage message:
            OnServerAfterReceivePerformCraftMessage(e.UniqueId, message);
            break;

          case ClientPerformDropItemMessage message:
            OnServerAfterReceivePerformDropItemMessage(e.UniqueId, message);
            break;

          case ClientPerformJumpMessage message:
            OnServerAfterReceivePerformJumpMessage(e.UniqueId, message);
            break;

          case ClientPerformLookAtMessage message:
            OnServerAfterReceivePerformLookAtMessage(e.UniqueId, message);
            break;

          case ClientPerformMergeSlotsMessage message:
            OnServerAfterReceivePerformMergeSlotsMessage(e.UniqueId, message);
            break;

          case ClientPerformMoveMessage message:
            OnServerAfterReceivePerformMoveMessage(e.UniqueId, message);
            break;

          case ClientPerformRotateMessage message:
            OnServerAfterReceivePerformRotateMessage(e.UniqueId, message);
            break;

          case ClientPerformSwapSlotsMessage message:
            OnServerAfterReceivePerformSwapSlotsMessage(e.UniqueId, message);
            break;

          case ClientPerformSwitchMainHandSlotMessage message:
            OnServerAfterReceivePerformSwitchMainHandSlotMessage(e.UniqueId, message);
            break;

          case ClientPerformUseMessage message:
            OnServerAfterReceivePerformUseMessage(e.UniqueId, message);
            break;
        }

        NovelCraft.Server.Recorder.AfterReceiveMessageEventRecord record = new() {
          Tick = _game.CurrentTick,
          Data = new() {
            UniqueId = e.UniqueId,
            Message = e.Message
          }
        };

        _recorder?.Record(record);

      } catch (Exception exception) {
        _logger.Error($"Error while handling message {e.Message.Type} from {e.UniqueId}: {exception.Message}");
      }
    };
  }

  private void OnServerAfterReceiveGetBlocksAndEntitiesMessage(int uniqueId, ClientGetBlocksAndEntitiesMessage message) {
    List<Game.Entity> entities = _game.GetAllEntities();

    ServerGetBlocksAndEntitiesMessage response = new() {
      Sections = (from request in message.RequestSectionList
                  let section = _game.GetSectionAt(new Game.Position<int>(
                    request.X, request.Y, request.Z
                  ))
                  select new ServerGetBlocksAndEntitiesMessage.SectionType {
                    Position = new() {
                      X = section.Position.X,
                      Y = section.Position.Y,
                      Z = section.Position.Z
                    },
                    Blocks = section.GetAllBlockIds()
                  }).ToList(),
      Entities = (from entity in entities
                  select new ServerGetBlocksAndEntitiesMessage.EntityType {
                    TypeId = entity.TypeId,
                    UniqueId = entity.UniqueId,
                    Position = new() {
                      X = entity.Position.X,
                      Y = entity.Position.Y,
                      Z = entity.Position.Z
                    },
                    Orientation = new() {
                      Yaw = entity.Orientation.Yaw,
                      Pitch = entity.Orientation.Pitch
                    }
                  }).ToList()
    };

    _server.Send(uniqueId, response);
  }

  private void OnServerAfterReceiveGetPlayerInfoMessage(int uniqueId, ClientGetPlayerInfoMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player == null) {
      // TODO: Send error message.
      return;
    }

    ServerGetPlayerInfoMessage response = new() {
      Health = player.Health ?? 0,
      Orientation = new() {
        Yaw = player.Orientation.Yaw,
        Pitch = player.Orientation.Pitch
      },
      Position = new() {
        X = player.Position.X,
        Y = player.Position.Y,
        Z = player.Position.Z
      },
      MainHand = player.Inventory.MainHandSlot,
      Inventory = (from i in Enumerable.Range(0, Game.Inventory.SlotCount)
                   select (
                    (player.Inventory[i] is null) ? null : new ServerGetPlayerInfoMessage.ItemStackType {
                      TypeId = player.Inventory[i]!.TypeId,
                      Count = player.Inventory[i]!.Count
                    })).ToList(),
      UniqueId = player.UniqueId
    };

    _server.Send(uniqueId, response);
  }

  private void OnServerAfterReceiveGetTickMessage(int uniqueId, ClientGetTickMessage message) {
    ServerGetTickMessage response = new() {
      Tick = _game.CurrentTick,
      TicksPerSecond = _game.TicksPerSecond
    };

    _server.Send(uniqueId, response);
  }

  private void OnServerAfterReceivePerformAttackMessage(int uniqueId, ClientPerformAttackMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Attack(message.AttackKind switch {
      ClientPerformAttackMessage.AttackType.AttackClick => Game.Player.InteractionKind.Click,
      ClientPerformAttackMessage.AttackType.HoldStart => Game.Player.InteractionKind.HoldStart,
      ClientPerformAttackMessage.AttackType.HoldEnd => Game.Player.InteractionKind.HoldEnd,
      _ => throw new NotImplementedException()
    }, _game.CurrentTick);
  }

  private void OnServerAfterReceivePerformCraftMessage(int uniqueId, ClientPerformCraftMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Craft(message.ItemIdSequence);
  }

  private void OnServerAfterReceivePerformDropItemMessage(int uniqueId, ClientPerformDropItemMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    // TODO: One item a time.
    foreach (var item in message.DropItems) {
      player.DropItem(item.Slot, item.Count);
    }
  }

  private void OnServerAfterReceivePerformJumpMessage(int uniqueId, ClientPerformJumpMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Jump();
  }

  private void OnServerAfterReceivePerformLookAtMessage(int uniqueId, ClientPerformLookAtMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.LookAt(new() {
      X = message.LookAtPosition.X,
      Y = message.LookAtPosition.Y,
      Z = message.LookAtPosition.Z
    });
  }

  private void OnServerAfterReceivePerformMergeSlotsMessage(int uniqueId, ClientPerformMergeSlotsMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Inventory.MergeSlots(message.FromSlot, message.ToSlot);
  }

  private void OnServerAfterReceivePerformMoveMessage(int uniqueId, ClientPerformMoveMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.SetMovement(message.DirectionType switch {
      ClientPerformMoveMessage.Direction.Forward => Game.Player.MovementDirection.Forward,
      ClientPerformMoveMessage.Direction.Backward => Game.Player.MovementDirection.Backward,
      ClientPerformMoveMessage.Direction.Left => Game.Player.MovementDirection.Left,
      ClientPerformMoveMessage.Direction.Right => Game.Player.MovementDirection.Right,
      ClientPerformMoveMessage.Direction.Stop => Game.Player.MovementDirection.Stopped,
      _ => throw new NotImplementedException()
    });
  }

  private void OnServerAfterReceivePerformRotateMessage(int uniqueId, ClientPerformRotateMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Orientation = new() {
      Yaw = message.Orientation.Yaw,
      Pitch = message.Orientation.Pitch
    };
  }

  private void OnServerAfterReceivePerformSwapSlotsMessage(int uniqueId, ClientPerformSwapSlotsMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Inventory.SwapSlots(message.SlotA, message.SlotB);
  }

  private void OnServerAfterReceivePerformSwitchMainHandSlotMessage(int uniqueId, ClientPerformSwitchMainHandSlotMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    player.Inventory.MainHandSlot = message.NewMainHand;
  }

  private void OnServerAfterReceivePerformUseMessage(int uniqueId, ClientPerformUseMessage message) {
    Game.Player? player = _game.GetPlayer(uniqueId);

    if (player is null) {
      // TODO: Send error message.
      return;
    }

    // TODO: Use item.
    player.Use(message.UseType switch {
      ClientPerformUseMessage.UseKind.UseClick => Game.Player.InteractionKind.Click,
      ClientPerformUseMessage.UseKind.UseStart => Game.Player.InteractionKind.HoldStart,
      ClientPerformUseMessage.UseKind.UseEnd => Game.Player.InteractionKind.HoldEnd,
      _ => throw new NotImplementedException()
    });
  }
}
