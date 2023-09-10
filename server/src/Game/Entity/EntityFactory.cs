namespace NovelCraft.Server.Game;

/// <summary>
/// EntityFactory creates entity objects and bind definitions to them.
/// </summary>
public class EntityFactory {
  #region Static, const and readonly fields
  #endregion


  #region Fields and properties
  private Dictionary<int, EntityDefinition> _definitionMap = new(); // Entity type ID -> Entity definition

  private UniqueIdGenerator _idGenerator = new();
  #endregion


  #region Constructors and finalizers
  public EntityFactory(List<EntityDefinition> definitionList) {
    foreach (EntityDefinition definition in definitionList) {
      _definitionMap.Add(definition.Description.TypeId, definition);
    }
  }

  public EntityFactory() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Creates a new entity.
  /// </summary>
  /// <param name="entityTypeId">The entity type ID</param>
  /// <param name="position">The position of the entity</param>
  /// <param name="tickCreated">The tick when the entity is created</param>
  /// <returns>The entity object</returns>
  public Entity CreateEntity(int entityTypeId, Position<decimal> position, int tickCreated) {
    if (!_definitionMap.ContainsKey(entityTypeId)) {
      throw new Exception($"Entity type ID {entityTypeId} is not registered.");
    }

    return entityTypeId switch {
      Player.PlayerTypeId => new Player(_definitionMap[entityTypeId], _idGenerator.Generate(), position, tickCreated),
      ItemEntity.ItemEntityTypeId => throw new Exception("ItemEntity should not be created by this method."),
      _ => new Entity(_definitionMap[entityTypeId], _idGenerator.Generate(), position, tickCreated),
    };
  }

  /// <summary>
  /// Creates a new item entity.
  /// </summary>
  /// <param name="itemStack">The item stack contained by the item entity</param>
  /// <param name="position">The position of the entity</param>
  /// <param name="tickCreated">The tick when the item entity is created</param>
  /// <returns>The entity object</returns>
  public ItemEntity CreateItemEntity(ItemStack itemStack, Position<decimal> position, int tickCreated) {
    if (!_definitionMap.ContainsKey(ItemEntity.ItemEntityTypeId)) {
      throw new Exception($"Entity type ID {ItemEntity.ItemEntityTypeId} is not registered.");
    }

    return new ItemEntity(_definitionMap[ItemEntity.ItemEntityTypeId], _idGenerator.Generate(), position, itemStack, tickCreated);
  }

  /// <summary>
  /// Registers a entity definition.
  /// </summary>
  /// <param name="definition">The entity definition</param>
  public void RegisterDefinition(EntityDefinition definition) {
    _definitionMap.Add(definition.Description.TypeId, definition);
  }

  /// <summary>
  /// Registers a list of entity definitions.
  /// </summary>
  /// <param name="definitionList">The list of entity definitions</param>
  public void RegisterDefinition(List<EntityDefinition> definitionList) {
    foreach (EntityDefinition definition in definitionList) {
      RegisterDefinition(definition);
    }
  }
  #endregion
}
