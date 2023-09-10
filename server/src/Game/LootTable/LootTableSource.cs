namespace NovelCraft.Server.Game;

public class LootTableSource {
  #region Fields and properties
  private ItemStackFactory _itemStackFactory;
  private List<LootTable> _lootTableList = new();
  #endregion


  #region Constructors and finalizers
  public LootTableSource(ItemStackFactory itemStackFactory) {
    _itemStackFactory = itemStackFactory;
  }
  #endregion


  #region Methods
  /// <summary>
  /// Generates a list of items from the loot table of a block.
  /// </summary>
  /// <param name="block">The block.</param>
  /// <param name="tool">The tool used to break the block.</param>
  /// <returns>A list of items.</returns>
  public List<ItemStack> GenerateBlockLoot(Block block, ItemStack? tool = null) {
    var itemStackList = new List<ItemStack>();

    foreach (LootTable lootTable in _lootTableList) {
      if (lootTable.Type is not LootTable.LootTableType.Block || lootTable.BlockTypeId != block.TypeId || !lootTable.CanLootBy(tool)) {
        continue;
      }

      var items = lootTable.Generate();

      foreach (var item in items) {
        itemStackList.Add(_itemStackFactory.CreateItemStack(item.ItemTypeId, item.Count));
      }
    }

    return itemStackList;
  }

  /// <summary>
  /// Generates a list of items from the loot table of an entity.
  /// </summary>
  /// <param name="entity">The entity.</param>
  /// <param name="tool">The tool used to break the block.</param>
  /// <returns>A list of items.</returns>
  public List<ItemStack> GenerateEntityLoot(Entity entity, ItemStack? tool = null) {
    var itemStackList = new List<ItemStack>();

    foreach (LootTable lootTable in _lootTableList) {
      if (lootTable.Type is not LootTable.LootTableType.Entity || lootTable.EntityTypeId != entity.TypeId || !lootTable.CanLootBy(tool)) {
        continue;
      }

      var items = lootTable.Generate();

      foreach (var item in items) {
        itemStackList.Add(_itemStackFactory.CreateItemStack(item.ItemTypeId, item.Count));
      }
    }

    return itemStackList;
  }

  /// <summary>
  /// Registers a loot table definition.
  /// </summary>
  /// <param name="definition">The loot table definition.</param>
  public void RegisterDefinition(LootTableDefinition definition) {
    LootTable lootTable = new(definition);
    this._lootTableList.Add(lootTable);
  }

  /// <summary>
  /// Registers a list of loot table definitions.
  /// </summary>
  /// <param name="definitionList">The list of loot table definitions.</param>
  public void RegisterDefinition(List<LootTableDefinition> definitionList) {
    foreach (LootTableDefinition definition in definitionList) {
      this.RegisterDefinition(definition);
    }
  }
  #endregion
}