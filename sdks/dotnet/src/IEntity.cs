namespace NovelCraft.Sdk;

/// <summary>
/// Represents an entity in the game world.
/// </summary>
public interface IEntity {
  /// <summary>
  /// Gets the orientation of the entity.
  /// </summary>
  public IOrientation Orientation { get; }

  /// <summary>
  /// Gets the position of the entity.
  /// </summary>
  public IPosition<decimal> Position { get; }

  /// <summary>
  /// Gets the type of the entity.
  /// </summary>
  public int TypeId { get; }

  /// <summary>
  /// Gets the unique ID of the entity.
  /// </summary>
  public int UniqueId { get; }
}