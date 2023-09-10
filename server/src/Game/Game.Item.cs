namespace NovelCraft.Server.Game;

public partial class Game {
  #region Fields and properties
  public ItemStackFactory ItemStackFactory = new();
  #endregion


  #region Methods
  /// <summary>
  /// CreateItemStack method creates an item stack.
  /// </summary>
  /// <param name="itemTypeId">The item type ID</param>
  /// <param name="amount">The amount of the item</param>
  /// <returns>The item stack object</returns>
  public ItemStack CreateItemStack(int itemTypeId, int amount) {
    return ItemStackFactory.CreateItemStack(itemTypeId, amount);
  }
  #endregion
}
