namespace NovelCraft.Server.Game;

public partial class Inventory {
  public class AfterChangeEventArgs : EventArgs {
    public Inventory Inventory { get; }
    public List<int> ChangedSlots { get; }

    public AfterChangeEventArgs(Inventory inventory, List<int> changedSlots) {
      this.Inventory = inventory;
      this.ChangedSlots = changedSlots;
    }
  }

  public event EventHandler<AfterChangeEventArgs>? AfterChangeEvent;
}