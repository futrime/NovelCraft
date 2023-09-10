namespace NovelCraft.Sdk;

/// <summary>
/// Represents a collection of entities.
/// </summary>
public interface IEntitySource : IEnumerable<IEntity> {
  /// <summary>
  /// Gets the entity with the unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID of the entity.</param>
  public IEntity? this[int uniqueId] { get; }


  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <returns>The list of all entities.</returns>
  public List<IEntity> GetAllEntities();
}