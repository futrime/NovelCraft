namespace NovelCraft.Server.Game;

public class EntitySource {
  #region Fields and properties
  private EntityFactory _entityFactory = new();

  public Dictionary<int, Entity> _entityMap { get; } = new(); // Unique ID -> Entity
  #endregion


  #region Constructors and finalizers
  public EntitySource(List<Entity> entityList) {
    foreach (Entity entity in entityList) {
      _entityMap.Add(entity.UniqueId, entity);
    }
  }

  public EntitySource() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Creates an entity.
  /// </summary>
  /// <param name="typeId">The type ID of the entity.</param>
  /// <param name="position">The position of the entity.</param>
  /// <param name="tickCreated">The tick when the entity is created.</param>
  /// <returns>The unique ID of the entity.</returns>
  public int CreateEntity(int typeId, Position<decimal> position, int tickCreated) {
    Entity entity = _entityFactory.CreateEntity(typeId, position, tickCreated);
    lock (_entityMap) {
      _entityMap.Add(entity.UniqueId, entity);
    }
    return entity.UniqueId;
  }

  /// <summary>
  /// Creates an item entity.
  /// </summary>
  /// <param name="itemStack">The item stack contained by the item entity</param>
  /// <param name="position">The position of the entity.</param>
  /// <param name="tickCreated">The tick when the item entity is created.</param>
  /// <returns>The unique ID of the entity.</returns>
  public int CreateItemEntity(ItemStack itemStack, Position<decimal> position, int tickCreated) {
    ItemEntity entity = _entityFactory.CreateItemEntity(itemStack, position, tickCreated);

    lock (_entityMap) {
      _entityMap.Add(entity.UniqueId, entity);
    }

    return entity.UniqueId;
  }

  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <returns>The list of entities.</returns>
  public List<Entity> GetAllEntities() {
    lock (_entityMap) {
      return _entityMap.Values.ToList();
    }
  }

  /// <summary>
  /// Gets an entity by its unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID of the entity.</param>
  /// <returns>The entity.</returns>
  public Entity? GetEntity(int uniqueId) {
    lock (_entityMap) {
      if (!_entityMap.ContainsKey(uniqueId)) {
        return null;
      }

      return _entityMap[uniqueId];
    }
  }

  /// <summary>
  /// Registers an entity definition.
  /// </summary>
  /// <param name="definition">The entity definition.</param>
  public void RegisterDefinition(EntityDefinition definition) {
    _entityFactory.RegisterDefinition(definition);
  }

  /// <summary>
  /// Registers a list of entity definitions.
  /// </summary>
  /// <param name="definitionList">The list of entity definitions.</param>
  public void RegisterDefinition(List<EntityDefinition> definitionList) {
    foreach (EntityDefinition definition in definitionList) {
      _entityFactory.RegisterDefinition(definition);
    }
  }

  /// <summary>
  /// Removes an entity.
  /// </summary>
  /// <param name="uniqueId">The unique ID of the entity.</param>
  public void RemoveEntity(int uniqueId) {
    lock (_entityMap) {
      if (!_entityMap.ContainsKey(uniqueId)) {
        return;
      }

      _entityMap.Remove(uniqueId);
    }
  }
  #endregion
}
