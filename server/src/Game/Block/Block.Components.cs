namespace NovelCraft.Server.Game;

public partial class Block {
  #region collision_box
  /// <summary>
  /// Gets the collision box of the block.
  /// </summary>
  public AABB? CollisionBox {
    get {
      // Check if the definition is loaded.
      if (_definition.Components.CollisionBox is null) {
        return null;
      }

      var position = Position;

      return new AABB(
        new Position<decimal>(
          position.X,
          position.Y,
          position.Z
        ),
        new SizeType<decimal>(1, 1, 1)
      );
    }
  }
  #endregion


  #region destructible_by_mining
  /// <summary>
  /// Gets the seconds to destroy the block.
  /// </summary>
  public decimal? SecondsToDestroy => _definition.Components.DestructibleByMining?.SecondsToDestroy;
  #endregion


  #region friction
  /// <summary>
  /// Gets the friction coefficient of the block.
  /// </summary>
  public decimal? FrictionCoefficient => _definition.Components.Friction;
  #endregion
}
