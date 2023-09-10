using System.Collections.Concurrent;

namespace NovelCraft.Server.Game;

public partial class Game {
  #region Nested classes, enums, delegates and events
  public struct AttackType {
    public Entity Attacker { get; init; }
    public Entity.InteractionKind AttackKind { get; init; }
  }
  public struct UseType {
    public Entity User { get; init; }
    public Entity.InteractionKind InteractionKind { get; init; }
  }
  public struct DiggingType {
    public Entity Digger { get; init; }
    public Position<int> TargetBlockPosition { get; init; }
    public int StartTick { get; init; }
  }

  public struct HurtType {
    public Entity Victim { get; init; }
    public decimal Damage { get; init; }
    public EntityDamageCause DamageCause { get; init; }
  }

  public struct PlayerInventoryChangeType {
    public Player Player { get; init; }
    public Inventory Inventory { get; init; }
    public List<int> ChangedSlots { get; init; }
  }


  public struct PlayerSWitchMainHandType {
    public Player Player { get; init; }
    public int NewMainHand { get; init; }
  }
  #endregion


  #region Static, const and readonly fields
  const int ItemPickupDelay = 20;
  const decimal ItemPickupRange = 2;
  #endregion


  #region Fields and properties
  private ConcurrentBag<AttackType> _attackInThisTickList = new();
  private ConcurrentBag<UseType> _useInThisTickList = new();
  private ConcurrentBag<Entity> _createdEntityInThisTickList = new();
  private ConcurrentBag<Entity> _despawnedEntityInThisTickList = new();
  private ConcurrentBag<HurtType> _hurtInThisTickList = new();
  private ConcurrentBag<Entity> _orientationChangedEntityInThisTickList = new();
  private ConcurrentBag<PlayerInventoryChangeType> _playerInventoryChangeInThisTickList = new();
  private ConcurrentBag<PlayerSWitchMainHandType> _playerSwitchMainHandInThisTickList = new();
  private ConcurrentBag<Entity> _positionChangedEntityInThisTickList = new();
  private ConcurrentBag<int> _removedEntityInThisTickList = new();
  private ConcurrentBag<Entity> _spawnedEntityInThisTickList = new();

  private ConcurrentDictionary<Entity, DiggingType> _diggingList = new();
  #endregion


  #region Methods
  /// <summary>
  /// Creates an entity.
  /// </summary>
  /// <param name="typeId">The entity ID</param>
  /// <param name="position">The position of the entity</param>
  /// <returns>The unique ID of the entity</returns>
  public int CreateEntity(int typeId, Position<decimal> position) {
    int uniqueId = _level.CreateEntity(typeId, position, CurrentTick);

    // Subscribe to entity events.
    SubscribeEntityEvents(_level.GetEntity(uniqueId)!);

    _createdEntityInThisTickList.Add(_level.GetEntity(uniqueId)!);

    return uniqueId;
  }

  /// <summary>
  /// Creates an item entity.
  /// </summary>
  /// <param name="itemStack">The item stack contained by the item entity</param>
  /// <param name="position">The position of the entity</param>
  /// <returns>The unique ID of the entity</returns>
  public int CreateItemEntity(ItemStack itemStack, Position<decimal> position) {
    int uniqueId = _level.CreateItemEntity(itemStack, position, CurrentTick);

    // Subscribe to entity events.
    SubscribeEntityEvents(_level.GetEntity(uniqueId)!);

    _createdEntityInThisTickList.Add(_level.GetEntity(uniqueId)!);

    return uniqueId;
  }

  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <returns>The list of entities</returns>
  public List<Entity> GetAllEntities() {
    return _level.GetAllEntities();
  }

  /// <summary>
  /// Gets all players.
  /// </summary>
  /// <returns>The list of players</returns>
  public List<Player> GetAllPlayers() {
    return _level.GetAllPlayers();
  }

  /// <summary>
  /// Gets the entity with the specified unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID</param>
  /// <returns>The entity</returns>
  public Entity? GetEntity(int uniqueId) {
    return _level.GetEntity(uniqueId);
  }

  /// <summary>
  /// Removes the entity with the specified unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID</param>
  public void RemoveEntity(int uniqueId) {
    _level.RemoveEntity(uniqueId);

    _removedEntityInThisTickList.Add(uniqueId);
  }


  private void OnEntityTryAttack(object? sender, Entity.TryAttackEventArgs e) {
    Entity attacker = e.Attacker;
    Entity.InteractionKind kind = e.Kind;

    _attackInThisTickList.Add(new AttackType() {
      Attacker = attacker,
      AttackKind = kind
    });

    (Entity? nearestLookedAtEntity, Position<decimal>? nearestEntityIntersectionPoint) = attacker.GetNearestLookedAtEntity(_level, 5);
    (Block? nearestLookedAtBlock, Position<decimal>? nearestBlockIntersectionPoint) = attacker.GetNearestLookedAtBlock(_level, 5);

    // If both are not null, then we need to determine which one is closer.
    if (nearestLookedAtEntity is not null && nearestLookedAtBlock is not null) {
      // Default: central position
      Position<decimal> entityPosition = nearestLookedAtEntity.Position;
      Position<decimal> blockPosition = nearestLookedAtBlock.Position;

      // If the intersection point exists, change the central position into intersection
      if (nearestEntityIntersectionPoint is not null) {
        entityPosition = nearestEntityIntersectionPoint;
      }
      if (nearestBlockIntersectionPoint is not null) {
        blockPosition = nearestBlockIntersectionPoint;
      }

      // Compare distance
      if (attacker.EyePosition.DistanceTo(entityPosition) < attacker.EyePosition.DistanceTo(blockPosition)) {
        nearestLookedAtBlock = null;
      } else {
        nearestLookedAtEntity = null;
      }
    }

    if (nearestLookedAtEntity is not null) { // Attack an entity.
      if (kind is Entity.InteractionKind.Click || kind is Entity.InteractionKind.HoldStart) {
        // Only attack if the entity has AttackDamage component.
        if (attacker.AttackDamage is not null) {
          decimal damage = attacker.AttackDamage.Value;

          if (attacker is Player player) {
            damage *= player.GetItemInSlot(player.GetMainHandSlot())?.AttackDamageMultiplier ?? 1;
          }

          nearestLookedAtEntity.Damage(damage, new EntityDamageCause(EntityDamageCause.KindType.EntityAttack, attacker), e.Tick);
        }
      }

    } else if (nearestLookedAtBlock is not null) { // Attack a block.
      switch (kind) {
        case Entity.InteractionKind.HoldStart:
          DiggingType digging = new() {
            Digger = attacker,
            TargetBlockPosition = nearestLookedAtBlock.Position,
            StartTick = CurrentTick
          };

          // If exists, then update the digging type.
          _diggingList.AddOrUpdate(attacker, digging, (key, oldValue) => digging);

          break;

        case Entity.InteractionKind.HoldEnd:
          // Find the digging type of the entity and remove it.
          _diggingList.TryRemove(attacker, out _);
          break;
      }

    } else { // Attack nothing.
      // Empty.
    }
  }
  private void OnEntityTryUse(object? sender, Entity.TryUseEventArgs e) {

    Entity entity = e.User;
    Entity.InteractionKind kind = e.Kind;

    // The entity must be player
    if (entity is not Player) {
      return;
    }

    Player user = ((Player)entity);
    ItemStack? mainHandSlot = user.GetItemInSlot(user.GetMainHandSlot());
    // Main hand should be not null or air and the count must be greater than 0 
    if (mainHandSlot is null || mainHandSlot.Count <= 0 || mainHandSlot.TypeId == 0) {
      return;
    }
    // Whether the main hand slot is food
    if (mainHandSlot.IsFood) {
      if (user.Health < user.MaxHealth || mainHandSlot.CanAlwaysEat is true) {
        decimal nutrition = mainHandSlot.Nutrition ?? 0;
        int count = user.Inventory.TryRemoveItemInSlot(user.GetMainHandSlot(), 1);
        if (count == 1) {
          // heal
          user.Heal(nutrition);
        }
        AfterEntityHealEvent?.Invoke(this, new AfterEntityHealEventArgs(CurrentTick, user, nutrition));
      }
      return;
    }

    _useInThisTickList.Add(new UseType() {
      User = user,
      InteractionKind = kind
    });

    (Block? nearestLookedAtBlock, Position<decimal>? nearestBlockIntersectionPoint) = user.GetNearestLookedAtBlock(_level, 5);

    if (nearestLookedAtBlock is null || nearestBlockIntersectionPoint is null) {
      return;
    }

    // Get block id
    int? blockTypeId = mainHandSlot.GetBlockIdIfPlaceOn(nearestLookedAtBlock, ItemStack.PlacementKind.Any);
    if (blockTypeId is null) {
      return;
    }

    // Main hand can be placed
    // Judge the direction of the intersection point:
    // Suppose that the central point of the block is O, and intersection point is P,and We can extend the vector OP by a small epsilon
    // Therefore, we can get the place position of the block
    const decimal epsilon = 1e-6m;

    Position<decimal> blockCentralPosition = nearestLookedAtBlock.Center;
    Position<int> extendedPosition = new(
      (int)((1 + epsilon) * nearestBlockIntersectionPoint.X - epsilon * blockCentralPosition.X),
      (int)((1 + epsilon) * nearestBlockIntersectionPoint.Y - epsilon * blockCentralPosition.Y),
      (int)((1 + epsilon) * nearestBlockIntersectionPoint.Z - epsilon * blockCentralPosition.Z)
    );

    // The block at the target position to be placed must be air
    Block targetBlock = GetBlock(extendedPosition);
    if (targetBlock is null || targetBlock.TypeId != 0) {
      return;
    }

    // If an entity is in the position of the target block, return
    foreach (var entityInfo in GetAllEntities()) {
      if (entityInfo.CollisionBox is not null) {
        if (AABB.Intersect(entityInfo.CollisionBox, new AABB(targetBlock.Position, new Position<int>(
          targetBlock.Position.X + 1,
        targetBlock.Position.Y + 1,
        targetBlock.Position.Z + 1))))
          return;
      }
    }

    SetBlock(extendedPosition, blockTypeId.Value);
    // The count of item decreases
    // Send Inventory Change Message
    user.Inventory.TryRemoveItemInSlot(user.GetMainHandSlot(), 1);

    AfterEntityPlaceBlockEvent?.Invoke(this, new AfterEntityPlaceBlockEventArgs(CurrentTick, user, GetBlock(extendedPosition)));
  }

  private void OnEntityAfterDespawn(object? sender, Entity.AfterDespawnEventArgs e) {
    try {
      _despawnedEntityInThisTickList.Add(e.Entity);

      List<ItemStack> drops = _lootTableSource.GenerateEntityLoot(e.Entity);

      foreach (ItemStack itemStack in drops) {
        CreateItemEntity(itemStack, e.Entity.Position);
      }

      if (e.Entity.TypeId == 0) {
        e.Entity.Teleport(GenerateSpawnPosition());
        e.Entity.Spawn();
      }

    } catch (Exception ex) {
      _logger.Error($"An exception occurred while despawning entity: {ex.Message}");
    }
  }

  private void OnEntityAfterOrientationChange(object? sender, Entity.AfterOrientationChangeEventArgs e) {
    _orientationChangedEntityInThisTickList.Add(e.Entity);

    if (e.Entity.IsAttackingHold) {
      UpdateEntityDigging(e.Entity);
    }
  }

  private void OnEntityAfterPositionChange(object? sender, Entity.AfterPositionChangeEventArgs e) {
    _positionChangedEntityInThisTickList.Add(e.Entity);

    if (e.Entity.IsAttackingHold) {
      UpdateEntityDigging(e.Entity);
    }
  }

  private void OnPlayerAfterDropItem(object? sender, Player.AfterDropItemEventArgs e) {
    CreateItemEntity(ItemStackFactory.CreateItemStack(e.ItemTypeIdDropped, e.amountDropped), e.Player.Position);
  }

  private void OnPlayerAfterPickupItem(object? sender, Player.AfterPickupItemEventArgs e) {
    // If the item stack is not picked up, then create an item entity.
    if (e.ItemStackNotPickedUp is not null) {
      CreateItemEntity(e.ItemStackNotPickedUp, e.Player.Position);
    }
  }

  private void OnPlayerTryCraft(object? sender, Player.TryCraftEventArgs e) {
    List<ItemStack?> ingredients = (
      from itemTypeId in e.Ingredients select ((itemTypeId is null) ? null : ItemStackFactory.CreateItemStack(itemTypeId.Value, 1))).ToList();
    List<ItemStack>? results = _recipeSource.Craft(ingredients);

    if (results is not null) {
      bool haveAllIngredients = true;
      foreach (ItemStack? itemStack in ingredients) {
        if (itemStack is not null && !e.Crafter.HasItem(itemStack)) {
          haveAllIngredients = false;
          break;
        }
      }

      if (!haveAllIngredients) {
        return;
      }

      foreach (ItemStack? itemStack in ingredients) {
        if (itemStack is not null) {
          e.Crafter.RemoveItem(itemStack.TypeId, itemStack.Count);
        }
      }

      foreach (ItemStack itemStack in results) {
        e.Crafter.GiveItem(itemStack);
      }
    }
  }

  /// <summary>
  /// Subscribes to events of an entity.
  /// </summary>
  private void SubscribeEntityEvents(Entity entity) {
    entity.TryAttackEvent += OnEntityTryAttack;
    entity.TryUseEvent += OnEntityTryUse;

    entity.AfterDespawnEvent += OnEntityAfterDespawn;

    entity.AfterHurtEvent += (object? sender, Entity.AfterHurtEventArgs e) => {
      _hurtInThisTickList.Add(new HurtType() {
        Victim = e.Victim,
        Damage = e.Damage,
        DamageCause = e.DamageCause
      });
    };

    entity.AfterOrientationChangeEvent += OnEntityAfterOrientationChange;

    entity.AfterPositionChangeEvent += OnEntityAfterPositionChange;

    entity.AfterSpawnEvent += (object? sender, Entity.AfterSpawnEventArgs e) => {
      _spawnedEntityInThisTickList.Add(e.Entity);
    };

    if (entity is Player player) {
      player.AfterDropItemEvent += OnPlayerAfterDropItem;

      player.AfterInventoryChangeEvent += (object? sender, Player.AfterInventoryChangeEventArgs e) => {
        _playerInventoryChangeInThisTickList.Add(new PlayerInventoryChangeType() {
          Player = player,
          Inventory = e.Inventory,
          ChangedSlots = e.ChangedSlots
        });
      };

      // player.AfterSwitchMainHandEvent += (object? sender, Player.AfterSwitchMainHandEventArgs e) => {
      //   _playerSwitchMainHandInThisTickList.Add(new PlayerSWitchMainHandType() {
      //     Player = player,
      //     NewMainHand = e.NewMainHand
      //   });
      // };

      player.AfterPickupItemEvent += OnPlayerAfterPickupItem;

      player.TryCraftEvent += OnPlayerTryCraft;
    }
  }

  /// <summary>
  /// Updates the entities.
  /// </summary>
  private void UpdateEntities() {
    foreach (Entity entity in _level.GetAllEntities()) {
      entity.Update(_level, DefaultTimeGap, CurrentTick);

      // Players can pick up items.
      if (entity is ItemEntity itemEntity && CurrentTick - itemEntity.SpawnTick >= ItemPickupDelay) {
        List<Player> players = GetAllPlayers();

        // Sort the players by distance to the item entity. The closest player will pick up first.
        players.Sort((a, b) => a.Position.DistanceTo(itemEntity.Position).CompareTo(b.Position.DistanceTo(itemEntity.Position)));

        foreach (Player player in players) {
          if (player.Position.DistanceTo(itemEntity.Position) <= ItemPickupRange && player.CanPickUpItem(itemEntity.ItemStack)) {
            player.PickUpItem(itemEntity.ItemStack);
            RemoveEntity(itemEntity.UniqueId);
            break;
          }
        }
      }
    }

    // AfterEntityCreateEvent and AfterEntitySpawnEvent are invoked first.
    if (_createdEntityInThisTickList.Count > 0) {
      AfterEntityCreateEvent?.Invoke(this, new AfterEntityCreateEventArgs(_createdEntityInThisTickList.ToList(), CurrentTick));
      _createdEntityInThisTickList.Clear();
    }

    if (_spawnedEntityInThisTickList.Count > 0) {
      AfterEntitySpawnEvent?.Invoke(this, new AfterEntitySpawnEventArgs(_spawnedEntityInThisTickList.ToList(), CurrentTick));
      _spawnedEntityInThisTickList.Clear();
    }

    if (_attackInThisTickList.Count > 0) {
      AfterEntityAttackEvent?.Invoke(this, new AfterEntityAttackEventArgs(_attackInThisTickList.ToList(), CurrentTick));
      _attackInThisTickList.Clear();
    }

    if (_hurtInThisTickList.Count > 0) {
      AfterEntityHurtEvent?.Invoke(this, new AfterEntityHurtEventArgs(_hurtInThisTickList.ToList(), CurrentTick));
      _hurtInThisTickList.Clear();
    }

    if (_orientationChangedEntityInThisTickList.Count > 0) {
      AfterEntityOrientationChangeEvent?.Invoke(this, new AfterEntityOrientationChangeEventArgs(_orientationChangedEntityInThisTickList.ToList(), CurrentTick));
      _orientationChangedEntityInThisTickList.Clear();
    }

    if (_playerInventoryChangeInThisTickList.Count > 0) {
      AfterPlayerInventoryChangeEvent?.Invoke(this, new AfterPlayerInventoryChangeEventArgs(_playerInventoryChangeInThisTickList.ToList(), CurrentTick));
      _playerInventoryChangeInThisTickList.Clear();
    }

    if (_playerSwitchMainHandInThisTickList.Count > 0) {
      AfterPlayerSwitchMainHandEvent?.Invoke(this, new AfterPlayerSwitchMainHandArgs(_playerSwitchMainHandInThisTickList.ToList(), CurrentTick));
      _playerSwitchMainHandInThisTickList.Clear();
    }

    if (_positionChangedEntityInThisTickList.Count > 0) {
      AfterEntityPositionChangeEvent?.Invoke(this, new AfterEntityPositionChangeEventArgs(_positionChangedEntityInThisTickList.ToList(), CurrentTick));
      _positionChangedEntityInThisTickList.Clear();
    }

    // AfterEntityDespawnEvent and AfterEntityRemoveEvent are invoked last.
    if (_despawnedEntityInThisTickList.Count > 0) {
      AfterEntityDespawnEvent?.Invoke(this, new AfterEntityDespawnEventArgs(_despawnedEntityInThisTickList.ToList(), CurrentTick));
      _despawnedEntityInThisTickList.Clear();
    }

    if (_removedEntityInThisTickList.Count > 0) {
      AfterEntityRemoveEvent?.Invoke(this, new AfterEntityRemoveEventArgs(_removedEntityInThisTickList.ToList(), CurrentTick));
      _removedEntityInThisTickList.Clear();
    }
  }

  /// <summary>
  /// Updates digging when an entity changes its position or orientation.
  /// </summary>
  /// <param name="entity">The entity.</param>
  private void UpdateEntityDigging(Entity entity) {
    if (!entity.IsAttackingHold) {
      return;
    }

    (Entity? nearestLookedAtEntity, _) = entity.GetNearestLookedAtEntity(_level, 5);
    (Block? nearestLookedAtBlock, _) = entity.GetNearestLookedAtBlock(_level, 5);

    // If both of them are not null, then we need to determine which one is closer.
    if (nearestLookedAtEntity is not null && nearestLookedAtBlock is not null) {
      if (entity.Position.DistanceTo(nearestLookedAtEntity.Position) < entity.Position.DistanceTo(nearestLookedAtBlock.Position)) {
        nearestLookedAtBlock = null;
      } else {
        nearestLookedAtEntity = null;
      }
    }

    if (nearestLookedAtBlock is null) {
      return;
    }

    bool isFoundInDiggingList = _diggingList.TryGetValue(entity, out DiggingType digging);

    if (isFoundInDiggingList && digging.TargetBlockPosition == nearestLookedAtBlock.Position) {
      return;
    }

    entity.Attack(Entity.InteractionKind.HoldEnd, CurrentTick);
    entity.Attack(Entity.InteractionKind.HoldStart, CurrentTick);
  }

  #endregion
}
