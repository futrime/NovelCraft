using NovelCraft.Utilities.Messages;

namespace NovelCraft.Server;

public partial class Binder {
  private void SubscribeToGame() {
    _game.AfterBlockChangeEvent += OnGameAfterBlockChangeEvent;
    _game.AfterEntityAttackEvent += OnGameAfterEntityAttackEvent;
    _game.AfterEntityBreakBlockEvent += OnGameAfterEntityBreakBlockEvent;
    _game.AfterEntityCreateEvent += OnGameAfterEntityCreateEvent;
    _game.AfterEntityDespawnEvent += OnGameAfterEntityDespawnEvent;
    _game.AfterEntityHealEvent += OnGameAfterEntityHealEvent;
    _game.AfterEntityHurtEvent += OnGameAfterEntityHurtEvent;
    _game.AfterEntityOrientationChangeEvent += OnGameAfterEntityOrientationChangeEvent;
    _game.AfterEntityPlaceBlockEvent += OnGameAfterEntityPlaceBlockEvent;
    _game.AfterEntityPositionChangeEvent += OnGameAfterEntityPositionChangeEvent;
    _game.AfterEntityRemoveEvent += OnGameAfterEntityRemoveEvent;
    _game.AfterEntitySpawnEvent += OnGameAfterEntitySpawnEvent;
    _game.AfterPlayerInventoryChangeEvent += OnGameAfterPlayerInventoryChangeEvent;
    _game.AfterPlayerSwitchMainHandEvent += OnGameAfterPlayerSwitchMainHandEvent;
  }

  private void OnGameAfterBlockChangeEvent(object? sender, NovelCraft.Server.Game.AfterBlockChangeEventArgs e) {
    ServerAfterBlockChangeMessage message = new() {
      ChangeList = (from pair in e.BlockChangeList
                    select new ServerAfterBlockChangeMessage.ChangeType {
                      Position = new() {
                        X = pair.Key.X,
                        Y = pair.Key.Y,
                        Z = pair.Key.Z
                      },
                      BlockTypeId = pair.Value
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterBlockChangeEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        ChangeList = (from pair in e.BlockChangeList
                      select new NovelCraft.Server.Recorder.AfterBlockChangeEventRecord.ChangeType {
                        Position = new() {
                          X = pair.Key.X,
                          Y = pair.Key.Y,
                          Z = pair.Key.Z
                        },
                        BlockTypeId = pair.Value
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityAttackEvent(object? sender, NovelCraft.Server.Game.AfterEntityAttackEventArgs e) {
    ServerAfterEntityAttackMessage message = new() {
      AttackList = (from attack in e.AttackList
                    select new ServerAfterEntityAttackMessage.AttackType {
                      AttackerUniqueId = attack.Attacker.UniqueId,
                      Kind = attack.AttackKind switch {
                        NovelCraft.Server.Game.Entity.InteractionKind.Click => ServerAfterEntityAttackMessage.AttackType.AttackKind.Click,
                        NovelCraft.Server.Game.Entity.InteractionKind.HoldStart => ServerAfterEntityAttackMessage.AttackType.AttackKind.HoldStart,
                        NovelCraft.Server.Game.Entity.InteractionKind.HoldEnd => ServerAfterEntityAttackMessage.AttackType.AttackKind.HoldEnd,
                        _ => throw new ArgumentOutOfRangeException()
                      }
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityAttackEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        AttackList = (from attack in e.AttackList
                      select new NovelCraft.Server.Recorder.AfterEntityAttackEventRecord.AttackType {
                        AttackerUniqueId = attack.Attacker.UniqueId,
                        AttackKind = attack.AttackKind switch {
                          NovelCraft.Server.Game.Entity.InteractionKind.Click => "click",
                          NovelCraft.Server.Game.Entity.InteractionKind.HoldStart => "hold_start",
                          NovelCraft.Server.Game.Entity.InteractionKind.HoldEnd => "hold_end",
                          _ => throw new ArgumentOutOfRangeException()
                        }
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityBreakBlockEvent(object? sender, NovelCraft.Server.Game.AfterEntityBreakBlockEventArgs e) {
    NovelCraft.Server.Recorder.AfterEntityBreakBlockEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        EntityUniqueId = e.Entity.UniqueId,
        BlockTypeId = e.Block.TypeId,
        BlockPosition = new() {
          X = e.Block.Position.X,
          Y = e.Block.Position.Y,
          Z = e.Block.Position.Z
        }
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityCreateEvent(object? sender, NovelCraft.Server.Game.AfterEntityCreateEventArgs e) {
    ServerAfterEntityCreateMessage message = new() {
      CreationList = (from entity in e.EntityList
                      select new ServerAfterEntityCreateMessage.CreationType {
                        UniqueId = entity.UniqueId,
                        EntityTypeId = entity.TypeId,
                        Position = new() {
                          X = entity.Position.X,
                          Y = entity.Position.Y,
                          Z = entity.Position.Z
                        },
                        Orientation = new() {
                          Yaw = entity.Orientation.Yaw,
                          Pitch = entity.Orientation.Pitch
                        },
                        ItemTypeId = (entity is NovelCraft.Server.Game.ItemEntity itemEntity) ? itemEntity.ItemStack.TypeId : null,
                        Health = entity.Health
                      }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityCreateEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        CreationList = (from entity in e.EntityList
                        select new NovelCraft.Server.Recorder.AfterEntityCreateEventRecord.CreationType {
                          UniqueId = entity.UniqueId,
                          EntityTypeId = entity.TypeId,
                          Position = new() {
                            X = entity.Position.X,
                            Y = entity.Position.Y,
                            Z = entity.Position.Z
                          },
                          Orientation = new() {
                            Yaw = entity.Orientation.Yaw,
                            Pitch = entity.Orientation.Pitch
                          },
                          ItemTypeId = (entity is NovelCraft.Server.Game.ItemEntity itemEntity) ? itemEntity.ItemStack.TypeId : null,
                          Health = (entity.Health is not null && entity.IsDead is not null) ? new() {
                            Health = entity.Health.Value,
                            IsDead = entity.IsDead.Value
                          } : null
                        }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityDespawnEvent(object? sender, NovelCraft.Server.Game.AfterEntityDespawnEventArgs e) {
    ServerAfterEntityDespawnMessage message = new() {
      DespawnIdList = (from entity in e.EntityList
                       select entity.UniqueId).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityDespawnEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        DespawnList = (from entity in e.EntityList
                       select new NovelCraft.Server.Recorder.AfterEntityDespawnEventRecord.DespawnType {
                         UniqueId = entity.UniqueId
                       }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityHealEvent(object? sender, NovelCraft.Server.Game.AfterEntityHealEventArgs e) {
    try {
      NovelCraft.Server.Recorder.AfterEntityHealEventRecord record = new() {
        Tick = e.CurrentTick,
        Data = new() {
          EntityUniqueId = e.Entity.UniqueId,
          HealAmount = e.HealAmount,
          Health = e.Entity.Health!.Value
        }
      };

      _recorder?.Record(record);

    } catch (Exception ex) {
      _logger.Error($"Failed to record after entity heal event: {ex.Message}");
    }
  }

  private void OnGameAfterEntityHurtEvent(object? sender, NovelCraft.Server.Game.AfterEntityHurtEventArgs e) {
    ServerAfterEntityHurtMessage message = new() {
      HurtList = (from hurt in e.HurtList
                  select new ServerAfterEntityHurtMessage.HurtType {
                    VictimUniqueId = hurt.Victim.UniqueId,
                    Damage = hurt.Damage,
                    Health = hurt.Victim.Health,
                    IsDead = hurt.Victim.IsDead
                  }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityHurtEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        HurtList = (from hurt in e.HurtList
                    select new NovelCraft.Server.Recorder.AfterEntityHurtEventRecord.HurtType {
                      VictimUniqueId = hurt.Victim.UniqueId,
                      Damage = hurt.Damage,
                      Health = hurt.Victim.Health!.Value,
                      IsDead = hurt.Victim.IsDead!.Value,
                      DamageCause = new Recorder.AfterEntityHurtEventRecord.HurtType.DamageCauseType {
                        Kind = ((int)hurt.DamageCause.Kind),
                        AttackerUniqueId = hurt.DamageCause.Attacker?.UniqueId,
                      }
                    }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityOrientationChangeEvent(object? sender, NovelCraft.Server.Game.AfterEntityOrientationChangeEventArgs e) {
    ServerAfterEntityOrientationChangeMessage message = new() {
      ChangeList = (from entity in e.EntityList
                    select new ServerAfterEntityOrientationChangeMessage.ChangeType {
                      UniqueId = entity.UniqueId,
                      Orientation = new() {
                        Yaw = entity.Orientation.Yaw,
                        Pitch = entity.Orientation.Pitch
                      }
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityOrientationChangeEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        ChangeList = (from entity in e.EntityList
                      select new NovelCraft.Server.Recorder.AfterEntityOrientationChangeEventRecord.ChangeType {
                        UniqueId = entity.UniqueId,
                        Orientation = new() {
                          Yaw = entity.Orientation.Yaw,
                          Pitch = entity.Orientation.Pitch
                        }
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityPlaceBlockEvent(object? sender, NovelCraft.Server.Game.AfterEntityPlaceBlockEventArgs e) {
    NovelCraft.Server.Recorder.AfterEntityPlaceBlockEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        EntityUniqueId = e.Entity.UniqueId,
        BlockTypeId = e.Block.TypeId,
        BlockPosition = new() {
          X = e.Block.Position.X,
          Y = e.Block.Position.Y,
          Z = e.Block.Position.Z
        }
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityPositionChangeEvent(object? sender, NovelCraft.Server.Game.AfterEntityPositionChangeEventArgs e) {
    ServerAfterEntityPositionChangeMessage message = new() {
      ChangeList = (from entity in e.EntityList
                    select new ServerAfterEntityPositionChangeMessage.ChangeType {
                      UniqueId = entity.UniqueId,
                      Position = new() {
                        X = entity.Position.X,
                        Y = entity.Position.Y,
                        Z = entity.Position.Z
                      },
                      Velocity = new() {
                        X = entity.Velocity.X,
                        Y = entity.Velocity.Y,
                        Z = entity.Velocity.Z
                      }
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityPositionChangeEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        ChangeList = (from entity in e.EntityList
                      select new NovelCraft.Server.Recorder.AfterEntityPositionChangeEventRecord.ChangeType {
                        UniqueId = entity.UniqueId,
                        Position = new() {
                          X = entity.Position.X,
                          Y = entity.Position.Y,
                          Z = entity.Position.Z
                        }
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntityRemoveEvent(object? sender, NovelCraft.Server.Game.AfterEntityRemoveEventArgs e) {
    ServerAfterEntityRemoveMessage message = new() {
      RemovalIdList = e.UniqueIdList
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntityRemoveEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        RemovalList = (from uniqueId in e.UniqueIdList
                       select new NovelCraft.Server.Recorder.AfterEntityRemoveEventRecord.RemovalType {
                         UniqueId = uniqueId
                       }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterEntitySpawnEvent(object? sender, NovelCraft.Server.Game.AfterEntitySpawnEventArgs e) {
    ServerAfterEntitySpawnMessage message = new() {
      SpawnIdList = (from entity in e.EntityList
                     select entity.UniqueId).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterEntitySpawnEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        SpawnList = (from entity in e.EntityList
                     select new NovelCraft.Server.Recorder.AfterEntitySpawnEventRecord.SpawnType {
                       UniqueId = entity.UniqueId
                     }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterPlayerInventoryChangeEvent(object? sender, NovelCraft.Server.Game.AfterPlayerInventoryChangeEventArgs e) {
    ServerAfterPlayerInventoryChangeMessage message = new() {
      ChangeList = (from change in e.ChangeList
                    select new ServerAfterPlayerInventoryChangeMessage.ChangeType {
                      PlayerUniqueId = change.Player.UniqueId,
                      ChangeList = (from slot in change.ChangedSlots
                                    select new ServerAfterPlayerInventoryChangeMessage.ChangeType.InventoryChangeType() {
                                      Slot = slot,
                                      ItemTypeId = change.Inventory[slot]?.TypeId,
                                      Count = change.Inventory[slot]?.Count ?? 0
                                    }).ToList()
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterPlayerInventoryChangeEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        ChangeList = (from change in e.ChangeList
                      select new NovelCraft.Server.Recorder.AfterPlayerInventoryChangeEventRecord.ChangeType {
                        PlayerUniqueId = change.Player.UniqueId,
                        ChangeList = (from slot in change.ChangedSlots
                                      select new NovelCraft.Server.Recorder.AfterPlayerInventoryChangeEventRecord.ChangeType.InventoryChangeType() {
                                        Slot = slot,
                                        ItemTypeId = change.Inventory[slot]?.TypeId,
                                        Count = change.Inventory[slot]?.Count ?? 0
                                      }).ToList()
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }

  private void OnGameAfterPlayerSwitchMainHandEvent(object? sender, NovelCraft.Server.Game.AfterPlayerSwitchMainHandArgs e) {
    ServerAfterPlayerSwitchMainHandMessage message = new() {
      ChangeList = (from change in e.ChangeList
                    select new ServerAfterPlayerSwitchMainHandMessage.ChangeType {
                      PlayerUniqueId = change.Player.UniqueId,
                      NewMainHand = change.NewMainHand
                    }).ToList()
    };

    _server.Broadcast(message);

    NovelCraft.Server.Recorder.AfterPlayerSwitchMainHandEventRecord record = new() {
      Tick = e.CurrentTick,
      Data = new() {
        ChangeList = (from change in e.ChangeList
                      select new NovelCraft.Server.Recorder.AfterPlayerSwitchMainHandEventRecord.ChangeType {
                        PlayerUniqueId = change.Player.UniqueId,
                        NewMainHand = change.NewMainHand
                      }).ToList()
      }
    };

    _recorder?.Record(record);
  }
}
