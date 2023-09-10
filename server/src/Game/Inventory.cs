namespace NovelCraft.Server.Game;

/// <summary>
/// Inventory class is the class for inventory for players;
/// </summary>
public partial class Inventory {
  #region Static, const and readonly fields
  public const int HotBarSlotCount = 9;
  public const int SlotCount = 36;
  #endregion


  #region Fields and properties
  public ItemStack? this[int index] {
    get {
      if (index < 0 || index >= Inventory.SlotCount) {
        throw new ArgumentOutOfRangeException("Slot is out of range.");
      }

      return this._itemStackList[index];
    }
    set {
      if (index < 0 || index >= Inventory.SlotCount) {
        throw new ArgumentOutOfRangeException("Slot is out of range.");
      }

      this._itemStackList[index] = value;

      this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, new List<int>() { index }));
    }
  }

  public int MainHandSlot {
    get => this._mainHandSlot;
    set {
      if (value < 0 || value >= Inventory.SlotCount) {
        throw new ArgumentOutOfRangeException("Slot is out of range.");
      }

      this._mainHandSlot = value;
    }
  }

  private List<ItemStack?> _itemStackList = Enumerable.Repeat<ItemStack?>(null, Inventory.SlotCount).ToList();
  private int _mainHandSlot = 0;
  #endregion


  #region Constructors
  public Inventory() {
    // Empty.
  }
  #endregion

  #region Methods
  /// <summary>
  /// Clears the inventory.
  /// </summary>
  public void Clear() {
    for (int i = 0; i < Inventory.SlotCount; i++) {
      this._itemStackList[i] = null;
    }
  }

  /// <summary>
  /// Collects two slots with the same item type and put them into one slot.
  /// If the item stack in the first slot is full, the items left will be put into the second slot.
  /// </summary>
  /// <param name="from">The slot to collect from.</param>
  /// <param name="to">The slot to collect to.</param>
  public void MergeSlots(int from, int to) {
    if (from < 0 || from >= Inventory.SlotCount ||
        to < 0 || to >= Inventory.SlotCount) {
      throw new ArgumentOutOfRangeException("Slot is out of range.");
    }

    ItemStack? itemStackFrom = this._itemStackList[from];
    ItemStack? itemStackTo = this._itemStackList[to];

    if (itemStackFrom is null) {
      return;
    }

    if (itemStackTo is null) {
      this._itemStackList[to] = itemStackFrom;
      this._itemStackList[from] = null;

      this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, new List<int>() { from, to }));
      return;
    }

    if (itemStackFrom.TypeId == itemStackTo.TypeId) {
      int amountToAdd = Math.Min(itemStackTo.MaxStackSize - itemStackTo.Count, itemStackFrom.Count);

      if (amountToAdd == 0) {
        return;
      }

      itemStackTo.Count += amountToAdd;
      itemStackFrom.Count -= amountToAdd;

      if (itemStackFrom.Count == 0) {
        this._itemStackList[from] = null;
      }

      this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, new List<int>() { from, to }));
    }
  }

  /// <summary>
  /// Swaps two slots in the inventory.
  /// </summary>
  /// <param name="slotA">The first slot to swap.</param>
  /// <param name="slotB">The second slot to swap.</param>
  public void SwapSlots(int slotA, int slotB) {
    if (slotA < 0 || slotA >= Inventory.SlotCount ||
        slotB < 0 || slotB >= Inventory.SlotCount) {
      throw new ArgumentOutOfRangeException("Slot is out of range.");
    }

    // Swap the slots
    ItemStack? temp = this._itemStackList[slotA];
    this._itemStackList[slotA] = this._itemStackList[slotB];
    this._itemStackList[slotB] = temp;

    // Raise the change event
    this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, new List<int>() { slotA, slotB }));
  }

  /// <summary>
  /// Trys to add an item to the inventory. If the inventory is full, the item left will be returned.
  /// </summary>
  /// <param name="itemStack">The item to add</param>
  /// <returns>The item left if the inventory is full, otherwise null.</returns>
  public ItemStack? TryAddItem(ItemStack itemStack) {
    List<int> changedSlots = new List<int>();

    foreach (int i in Enumerable.Range(0, Inventory.SlotCount)) {
      ItemStack? itemStackInInventory = _itemStackList[i];

      if (itemStackInInventory is not null && itemStackInInventory.TypeId == itemStack.TypeId) {
        int amountToAdd = Math.Min(itemStackInInventory.MaxStackSize - itemStackInInventory.Count, itemStack.Count);
        itemStackInInventory.Count += amountToAdd;
        itemStack.Count -= amountToAdd;

        changedSlots.Add(i);

        if (itemStack.Count == 0) {
          this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, changedSlots));
          return null;
        }
      }
    }

    for (int i = 0; i < Inventory.SlotCount; i++) {
      if (this._itemStackList[i] is null) {
        this._itemStackList[i] = itemStack;
        changedSlots.Add(i);
        return null;
      }
    }

    this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, changedSlots));
    return itemStack;
  }

  /// <summary>
  /// Trys to remove items from the inventory.
  /// </summary>
  /// <param name="typeId">The type id of the item to remove.</param>
  /// <param name="count">The amount of items to remove.</param>
  /// <returns>The amount of items removed.</returns>
  public int TryRemoveItem(int typeId, int count) {
    List<int> changedSlots = new List<int>();
    int amountRemoved = 0;

    for (int i = 0; i < Inventory.SlotCount; i++) {
      ItemStack? itemStack = _itemStackList[i];

      if (itemStack is not null && itemStack.TypeId == typeId) {
        int amountToRemove = Math.Min(itemStack.Count, count);
        itemStack.Count -= amountToRemove;
        amountRemoved += amountToRemove;
        count -= amountToRemove;

        if (itemStack.Count == 0) {
          this._itemStackList[i] = null;
        }

        changedSlots.Add(i);

        if (count == 0) {
          break;
        }
      }
    }

    this.AfterChangeEvent?.Invoke(this, new AfterChangeEventArgs(this, changedSlots));

    return amountRemoved;
  }

  /// <summary>
  /// Trys to remove items from the inventory.
  /// </summary>
  /// <param name="slot">The slot to remove the item from.</param>
  /// <param name="count">The amount of items to remove.</param>
  /// <returns>The amount of items removed.</returns>
  public int TryRemoveItemInSlot(int slot, int count) {
    ItemStack? itemStack = this._itemStackList[slot];

    if (itemStack is null) {
      return 0;
    }

    return TryRemoveItem(itemStack.TypeId, count);
  }
  #endregion 
}