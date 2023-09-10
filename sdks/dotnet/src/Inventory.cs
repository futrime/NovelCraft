using NovelCraft.Utilities.Messages;

namespace NovelCraft.Sdk;

internal class Inventory : IInventory {
  public IItemStack? this[int index] {
    get {
      if (index < 0 || index >= Size) {
        throw new IndexOutOfRangeException();
      }

      return _itemStacks[index];
    }
    set {
      if (index < 0 || index >= Size) {
        throw new IndexOutOfRangeException();
      }

      _itemStacks[index] = (value is null ? null : new ItemStack(value.TypeId, value.Count));
    }
  }

  public int HotBarSize => 9;

  public int MainHandSlot {
    get => _mainHandSlot;
    set {
      if (value < 0 || value >= HotBarSize) {
        throw new IndexOutOfRangeException();
      }

      _mainHandSlot = value;

      Sdk.Client?.Send(new ClientPerformSwitchMainHandSlotMessage() {
        Token = Sdk.Agent?.Token ?? throw new InvalidOperationException(),
        NewMainHand = value
      });
    }
  }

  public int Size => 36;

  private List<ItemStack?> _itemStacks = Enumerable.Repeat<ItemStack?>(null, 36).ToList();
  private int _mainHandSlot = 0;


  public Inventory() {
    // Empty
  }


  public void Craft(List<int?> ingredients) {
    Sdk.Client?.Send(new ClientPerformCraftMessage() {
      Token = Sdk.Agent?.Token ?? throw new InvalidOperationException(),
      ItemIdSequence = ingredients
    });
  }

  public void DropItem(int slot, int count) {
    if (slot < 0 || slot >= Size) {
      throw new IndexOutOfRangeException();
    }

    Sdk.Client?.Send(new ClientPerformDropItemMessage() {
      Token = Sdk.Agent?.Token ?? throw new InvalidOperationException(),
      DropItems = new() {
        new() {
          Slot = slot,
          Count = count
        }
      }
    });
  }

  public void MergeSlots(int fromSlot, int toSlot) {
    if (fromSlot < 0 || fromSlot >= Size) {
      throw new IndexOutOfRangeException();
    }

    if (toSlot < 0 || toSlot >= Size) {
      throw new IndexOutOfRangeException();
    }

    Sdk.Client?.Send(new ClientPerformMergeSlotsMessage() {
      Token = Sdk.Agent?.Token ?? throw new InvalidOperationException(),
      FromSlot = fromSlot,
      ToSlot = toSlot
    });
  }

  public void SwapSlots(int slot1, int slot2) {
    if (slot1 < 0 || slot1 >= Size) {
      throw new IndexOutOfRangeException();
    }

    if (slot2 < 0 || slot2 >= Size) {
      throw new IndexOutOfRangeException();
    }

    Sdk.Client?.Send(new ClientPerformSwapSlotsMessage() {
      Token = Sdk.Agent?.Token ?? throw new InvalidOperationException(),
      SlotA = slot1,
      SlotB = slot2
    });
  }
}