namespace NovelCraft.Server.Game;

/// <summary>
/// ItemStackFactory creates item stack objects and bind definitions to them.
/// </summary>
public class ItemStackFactory {
  #region Fields and properties
  private Dictionary<int, ItemDefinition> _definitionMap = new(); // Item type ID -> Item definition
  #endregion


  #region Constructors and finalizers
  public ItemStackFactory(List<ItemDefinition> definitionList) {
    foreach (ItemDefinition definition in definitionList) {
      _definitionMap.Add(definition.Description.TypeId, definition);
    }
  }

  public ItemStackFactory() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Creates a new item stack.
  /// </summary>
  /// <param name="itemTypeId">The item type ID</param>
  /// <param name="amount">The amount of the item</param>
  /// <returns>The item stack object</returns>
  public ItemStack CreateItemStack(int itemTypeId, int amount) {
    if (!_definitionMap.ContainsKey(itemTypeId)) {
      throw new Exception($"Item type ID {itemTypeId} is not registered.");
    }

    return new ItemStack(_definitionMap[itemTypeId], amount);
  }

  /// <summary>
  /// Gets the item type ID from an item identifier.
  /// </summary>
  /// <param name="identifier">The item identifier</param>
  /// <returns>The item type ID</returns>
  public int GetItemTypeIdFromIdentifier(string identifier) {
    foreach (ItemDefinition definition in _definitionMap.Values) {
      if (definition.Description.Identifier == identifier) {
        return definition.Description.TypeId;
      }
    }

    throw new Exception($"Item identifier {identifier} is not registered.");
  }

  /// <summary>
  /// Registers an item definition.
  /// </summary>
  /// <param name="definition">The item definition</param>
  public void RegisterDefinition(ItemDefinition definition) {
    _definitionMap.Add(definition.Description.TypeId, definition);
  }

  /// <summary>
  /// Registers a list of item definitions.
  /// </summary>
  /// <param name="definitionList">The list of item definitions</param>
  public void RegisterDefinition(List<ItemDefinition> definitionList) {
    foreach (ItemDefinition definition in definitionList) {
      RegisterDefinition(definition);
    }
  }
  #endregion
}
