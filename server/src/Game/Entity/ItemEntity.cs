namespace NovelCraft.Server.Game;

/// <summary>
/// ItemEntity is the entity of an item when dropped.
/// </summary>
public class ItemEntity : Entity {
  #region Static, const and readonly fields
  public const int ItemEntityTypeId = 1;
  #endregion


  #region Fields and properties
  public ItemStack ItemStack { get; }
  #endregion


  #region Constructors and finalizers
  public ItemEntity(EntityDefinition definition, int uniqueId, Position<decimal> position, ItemStack itemStack, int tickCreated) : base(definition, uniqueId, position, tickCreated) {
    if (definition.Description.TypeId != ItemEntityTypeId) {
      throw new ArgumentException("ItemEntity must be created with an item entity definition.");
    }

    ItemStack = itemStack;
  }
  #endregion
}
