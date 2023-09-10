using System.Collections.Concurrent;

namespace NovelCraft.Server.Game;

public partial class Game {
  #region Static, const and readonly fields
  private const int EmptyBlockTypeId = 0;
  #endregion

  #region Fields and Properties
  private ConcurrentDictionary<Position<int>, int> _blockChangeMap = new(); // Position -> new block type ID
  #endregion


  #region Methods
  /// <summary>
  /// Gets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  public Block GetBlock(Position<int> position) {
    return _level.GetBlock(position);
  }

  /// <summary>
  /// Gets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  public Block GetBlock(Position<decimal> position) {
    return GetBlock(new Position<int> {
      X = (int)position.X,
      Y = (int)position.Y,
      Z = (int)position.Z
    });
  }

  /// <summary>
  /// Sets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  /// <param name="blockId">The block type ID</param>
  public void SetBlock(Position<int> position, int blockId) {
    _level.SetBlock(position, blockId);

    // If exists, update the block change map.
    _blockChangeMap.AddOrUpdate(position, key => blockId, (key, oldValue) => blockId);
  }

  /// <summary>
  /// Updates the blocks.
  /// </summary>
  private void UpdateBlocks() {
    // Iterate over the digging list.
    foreach (KeyValuePair<Entity, DiggingType> kvp in _diggingList) {
      DiggingType digging = kvp.Value;
      Entity digger = kvp.Key;
      Block target = GetBlock(digging.TargetBlockPosition);

      // If the block is not diggable, remove the digging.
      if (target is null) {
        _diggingList.TryRemove(kvp.Key, out _);
        continue;
      }

      decimal destroySpeedMultiplier = 1;

      if (digger is Player player) {
        destroySpeedMultiplier = player.GetItemInSlot(player.GetMainHandSlot())?.GetDestroySpeedFor(target) ?? 1;
      }

      // If the digging is finished, set the block to air.
      if ((CurrentTick - digging.StartTick) * destroySpeedMultiplier >= target.SecondsToDestroy * 20) {
        SetBlock(digging.TargetBlockPosition, EmptyBlockTypeId);
        _diggingList.TryRemove(kvp.Key, out _);

        AfterEntityBreakBlockEvent?.Invoke(this, new AfterEntityBreakBlockEventArgs(CurrentTick, digger, target));

        // If the block has a loot table, drop the items.
        List<ItemStack> drops = _lootTableSource.GenerateBlockLoot(target);

        foreach (ItemStack drop in drops) {
          CreateItemEntity(drop, new(target.Position.X + 0.5m, target.Position.Y + 0.5m, target.Position.Z + 0.5m));
        }
      }
    }

    if (_blockChangeMap.Count > 0) {
      AfterBlockChangeEvent?.Invoke(this, new AfterBlockChangeEventArgs(_blockChangeMap.ToArray().ToDictionary(kvp => kvp.Key, kvp => kvp.Value), CurrentTick));
      _blockChangeMap.Clear();
    }
  }
  #endregion
}
