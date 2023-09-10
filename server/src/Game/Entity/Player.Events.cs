namespace NovelCraft.Server.Game;

public partial class Player {
  public class AfterDropItemEventArgs : EventArgs {
    public Player Player { get; }
    public int ItemTypeIdDropped { get; }
    public int amountDropped { get; }

    public AfterDropItemEventArgs(Player player, int itemTypeIdDropped, int amountDropped) {
      Player = player;
      ItemTypeIdDropped = itemTypeIdDropped;
      this.amountDropped = amountDropped;
    }
  }

  public class AfterInventoryChangeEventArgs : EventArgs {
    public Player Player { get; }
    public Inventory Inventory { get; }
    public List<int> ChangedSlots { get; }

    public AfterInventoryChangeEventArgs(Player player, Inventory inventory, List<int> changedSlots) {
      Player = player;
      Inventory = inventory;
      ChangedSlots = changedSlots;
    }
  }

  public class AfterSwitchMainHandEventArgs : EventArgs {
    public Player Player { get; }

    public int NewMainHand { get; }

    public AfterSwitchMainHandEventArgs(Player player, int newMainHand) {
      Player = player;
      NewMainHand = newMainHand;
    }
  }

  public class AfterPickupItemEventArgs : EventArgs {
    public Player Player { get; }
    public ItemStack? ItemStackNotPickedUp { get; }

    public AfterPickupItemEventArgs(Player player, ItemStack? itemStackNotPickedUp) {
      Player = player;
      ItemStackNotPickedUp = itemStackNotPickedUp;
    }
  }

  public class TryCraftEventArgs : EventArgs {
    public Player Crafter { get; }
    public List<int?> Ingredients { get; }

    public TryCraftEventArgs(Player crafter, List<int?> ingredients) {
      Crafter = crafter;
      Ingredients = ingredients;
    }
  }

  public event EventHandler<AfterDropItemEventArgs>? AfterDropItemEvent; // TODO: drop items
  public event EventHandler<AfterInventoryChangeEventArgs>? AfterInventoryChangeEvent;
  // public event EventHandler<AfterSwitchMainHandEventArgs>? AfterSwitchMainHandEvent;
  public event EventHandler<AfterPickupItemEventArgs>? AfterPickupItemEvent;
  public event EventHandler<TryCraftEventArgs>? TryCraftEvent;
}
