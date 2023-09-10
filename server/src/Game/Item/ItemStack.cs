namespace NovelCraft.Server.Game;

/// <summary>
/// ItemStack class is the class that represents a stack of items.
/// </summary>
public partial class ItemStack {
  #region Static, const and readonly fields
  #endregion


  #region Fields and properties
  public int Count {
    get => _count;
    set {
      if (value < 0) {
        throw new Exception("Item count cannot be negative.");
      }

      if (value > MaxStackSize) {
        throw new Exception("Item count cannot be greater than the maximum stack size.");
      }

      _count = value;
    }
  }

  public string Identifier => _definition.Description.Identifier;

  public int MaxStackSize => (_definition.Components.MaxStackSize is not null) ? _definition.Components.MaxStackSize.Value : 64;

  public int TypeId => _definition.Description.TypeId;

  private int _count;
  private ItemDefinition _definition;
  #endregion


  #region Constructors and finalizers
  public ItemStack(ItemDefinition definition, int count) {
    _definition = definition;
    Count = count;
  }
  #endregion


  #region Methods
  #endregion
}