namespace NovelCraft.Server.Game;

public partial class Game {
  #region Fields and properties
  #endregion

  #region Methods
  /// <summary>
  /// RegisterDefinition method registers a definition.
  /// </summary>
  public void RegisterDefinition(IDefinition definition) {
    switch (definition.Type) {
      case "block":
        _level.RegisterDefinition((BlockDefinition)definition);
        break;

      case "entity":
        _level.RegisterDefinition((EntityDefinition)definition);
        break;

      case "item":
        ItemStackFactory.RegisterDefinition((ItemDefinition)definition);
        break;

      case "loot_table":
        _lootTableSource.RegisterDefinition((LootTableDefinition)definition);
        break;

      default:
        // Ignore.
        break;
    }
  }
  #endregion
}
