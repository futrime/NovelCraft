namespace NovelCraft.Server.Game;

/// <summary>
/// ItemStack class is the class that represents a stack of items.
/// </summary>
public partial class ItemStack {
  #region block_placer
  public enum PlacementKind {
    Any, Top, Bottom, Side
  }

  public int? GetBlockIdIfPlaceOn(Block block, PlacementKind kind) {
    if (_definition.Components.BlockPlacer is null) {
      return null;
    }

    if (_definition.Components.BlockPlacer.CanPlaceOn is null) {
      return _definition.Components.BlockPlacer.BlockTypeId;
    }

    foreach (ItemDefinition.ComponentsType.BlockPlacerType.PlacementType placement in _definition.Components.BlockPlacer.CanPlaceOn) {
      if (placement.BlockTypeId != block.TypeId) {
        continue;
      }

      PlacementKind k = placement.Kind switch {
        "any" => PlacementKind.Any,
        "top" => PlacementKind.Top,
        "bottom" => PlacementKind.Bottom,
        "side" => PlacementKind.Side,
        _ => throw new Exception($"Invalid placement kind: {placement.Kind}")
      };

      if (k == kind || k == PlacementKind.Any) {
        return _definition.Components.BlockPlacer.BlockTypeId;
      }
    }

    return null;
  }
  #endregion


  #region digger
  public bool IsDigger => _definition.Components.Digger is not null;

  /// <summary>
  /// Gets the destroy speed of the item for the given block.
  /// </summary>
  /// <param name="block">The block to get the destroy speed for.</param>
  /// <returns>The destroy speed of the item for the given block.</returns>
  public decimal GetDestroySpeedFor(Block block) {
    return GetDestroySpeedFor(block.TypeId);
  }

  /// <summary>
  /// Gets the destroy speed of the item for the given block type.
  /// </summary>
  /// <param name="blockTypeId">The block type ID to get the destroy speed for.</param>
  /// <returns>The destroy speed of the item for the given block type.</returns>
  public decimal GetDestroySpeedFor(int blockTypeId) {
    if (_definition.Components.Digger is null) {
      return 1.0m;
    }

    foreach (ItemDefinition.ComponentsType.DiggerType.DestroySpeedType destroySpeed in _definition.Components.Digger.DestroySpeeds) {
      if (destroySpeed.BlockTypeId == blockTypeId) {
        return destroySpeed.SpeedMultiplier;
      }
    }

    return 1.0m;
  }
  #endregion


  #region food
  public bool IsFood => _definition.Components.Food is not null;

  public bool? CanAlwaysEat => _definition.Components.Food?.CanAlwaysEat;

  public decimal? Nutrition => _definition.Components.Food?.Nutrition;
  #endregion


  #region weapon
  public bool IsWeapon => _definition.Components.Weapon is not null;

  /// <summary>
  /// Gets the additional damage the holder of the item does when attacking.
  /// </summary>
  public decimal AttackDamageMultiplier => _definition.Components.Weapon?.Damage ?? 0.0m;
  #endregion
}