namespace NovelCraft.Server.Game;

/// <summary>
/// Player class is the class for the player entity.
/// </summary>
public partial class Player : Entity {
  #region Nested classes, enums, delegates and events
  #endregion


  #region Static, const and readonly fields
  public const int PlayerTypeId = 0;
  #endregion


  #region Fields and properties
  public ref readonly Inventory Inventory => ref _inventory;

  private Inventory _inventory = new();
  #endregion


  #region Constructors and finalizers
  public Player(EntityDefinition definition, int uniqueId, Position<decimal> position, int tickCreated) : base(definition, uniqueId, position, tickCreated) {
    if (definition.Description.TypeId != PlayerTypeId) {
      throw new ArgumentException("Player must be created with an player definition.");
    }

    this._inventory.AfterChangeEvent += (sender, e) => {
      this.AfterInventoryChangeEvent?.Invoke(this, new AfterInventoryChangeEventArgs(this, this._inventory, e.ChangedSlots));
    };
  }
  #endregion


  #region Methods
  /// <summary>
  /// Checks if the player can pick up an item.
  /// </summary>
  /// <param name="itemStack">The item to check.</param>
  /// <returns>True if the player can pick up the item, false otherwise.</returns>
  public bool CanPickUpItem(ItemStack itemStack) {
    foreach (int i in Enumerable.Range(0, Inventory.SlotCount)) {
      ItemStack? itemStackInSlot = _inventory[i];

      if (itemStackInSlot == null) {
        return true;
      }

      if (itemStackInSlot.TypeId == itemStack.TypeId && itemStackInSlot.Count < itemStack.MaxStackSize) {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Crafts.
  /// </summary>
  /// <param name="ingredients">The ingredients</param>
  public void Craft(List<int?> ingredients) {
    TryCraftEvent?.Invoke(this, new TryCraftEventArgs(this, ingredients));
  }

  /// <summary>
  /// Drops items.
  /// </summary>
  /// <param name="typeId">The type id of the item to drop.</param>
  /// <param name="count">The amount of items to drop.</param>
  public void DropItem(int typeId, int count) {
    int amountDropped = _inventory.TryRemoveItem(typeId, count);

    if (amountDropped > 0) {
      AfterDropItemEvent?.Invoke(this, new AfterDropItemEventArgs(this, typeId, amountDropped));
    }
  }

  /// <summary>
  /// Drops items.
  /// </summary>
  /// <param name="slot">The slot to drop items from.</param>
  /// <param name="count">The amount of items to drop.</param>
  public void DropItemInSlot(int slot, int count) {
    ItemStack? itemStack = _inventory[slot];

    if (itemStack != null) {
      DropItem(itemStack.TypeId, count);
    }
  }

  /// <summary>
  /// Gets the item in a slot.
  /// </summary>
  /// <param name="slot">The slot to get the item from.</param>
  /// <returns>The item in the slot.</returns>
  public ItemStack? GetItemInSlot(int slot) {
    return _inventory[slot];
  }

  /// <summary>
  /// Gets the main hand slot.
  /// </summary>
  /// <returns>The main hand slot.</returns>
  public int GetMainHandSlot() {
    return _inventory.MainHandSlot;
  }

  /// <summary>
  /// Gives items to the player.
  /// </summary>
  /// <param name="itemStack">The item stack to give.</param>
  public void GiveItem(ItemStack itemStack) {
    _inventory.TryAddItem(itemStack);
  }

  /// <summary>
  /// Checks if the player has an item.
  /// </summary>
  /// <param name="typeId">The type id of the item to check.</param>
  /// <param name="count">The amount of items to check.</param>
  /// <returns>True if the player has the item, false otherwise.</returns>
  public bool HasItem(ItemStack itemStackToCheck) {
    foreach (int i in Enumerable.Range(0, Inventory.SlotCount)) {
      ItemStack? itemStack = _inventory[i];

      if (itemStack != null && itemStack.TypeId == itemStackToCheck.TypeId && itemStack.Count >= itemStackToCheck.Count) {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Picks up items.
  /// </summary>
  /// <param name="itemStack">The item stack to pick up.</param>
  public void PickUpItem(ItemStack itemStack) {
    ItemStack? itemsNotPickedUp = _inventory.TryAddItem(itemStack);

    AfterPickupItemEvent?.Invoke(this, new AfterPickupItemEventArgs(this, itemsNotPickedUp));
  }

  /// <summary>
  /// Swaps two slots in the inventory.
  /// </summary>
  public void SwapInventorySlots(int slotA, int slotB) {
    _inventory.SwapSlots(slotA, slotB);
  }

  /// <summary>
  /// Removes items from the inventory.
  /// </summary>
  /// <param name="typeId">The type id of the item to remove.</param>
  /// <param name="count">The amount of items to remove.</param>
  public void RemoveItem(int typeId, int count) {
    _inventory.TryRemoveItem(typeId, count);
  }
  #endregion
}
