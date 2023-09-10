namespace NovelCraft.Server.Game;

public class LootTable {
  #region Nested classes, enums, delegates and events
  public enum LootTableType { Block, Entity }

  public record LootItemType {
    public required int ItemTypeId;
    public required int Count;
  }
  #endregion


  #region Fields and properties
  public LootTableType Type => _definition.Description.Type switch {
    "block" => LootTableType.Block,
    "entity" => LootTableType.Entity,
    _ => throw new ArgumentOutOfRangeException()
  };

  public int? BlockTypeId => (Type == LootTableType.Block ? _definition.Description.BlockOrEntityTypeId : null);

  public int? EntityTypeId => (Type == LootTableType.Entity ? _definition.Description.BlockOrEntityTypeId : null);

  private LootTableDefinition _definition;
  private Random _random = new();
  #endregion


  #region Constructors and finalizers
  public LootTable(LootTableDefinition definition) {
    _definition = definition;
  }
  #endregion


  #region Methods
  /// <summary>
  /// Generates a list of items from the loot table.
  /// </summary>
  /// <returns>A list of items.</returns>
  public List<LootItemType> Generate() {
    var items = new List<LootItemType>();

    foreach (var pool in _definition.Pools) {
      int rolls = pool.Rolls;

      for (int i = 0; i < rolls; i++) {
        int totalWeight = pool.Entries.Sum(entry => entry.Weight);
        int randomWeight = _random.Next(0, totalWeight);

        foreach (var entry in pool.Entries) {
          if (randomWeight < entry.Weight) {
            if (entry.ItemTypeId is not null) {
              items.Add(new LootItemType {
                ItemTypeId = entry.ItemTypeId.Value,
                Count = 1
              });
            }

            break;
          }

          randomWeight -= entry.Weight;
        }
      }
    }

    return items;
  }

  /// <summary>
  /// Checks if the loot table can be looted by the specified tool.
  /// </summary>
  /// <param name="tool">The tool.</param>
  /// <returns>True if the loot table can be looted by the specified tool, otherwise false.</returns>
  public bool CanLootBy(ItemStack? tool) {
    if (_definition.OnlyLootBy is null) {
      return true;
    }

    if (tool is null) {
      return false;
    }

    return _definition.OnlyLootBy.Contains(tool.TypeId);
  }
  #endregion
}
