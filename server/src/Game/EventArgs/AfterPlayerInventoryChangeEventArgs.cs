namespace NovelCraft.Server.Game;

public class AfterPlayerInventoryChangeEventArgs : EventArgs {
  public List<Game.PlayerInventoryChangeType> ChangeList { get; }
  public int CurrentTick { get; }

  public AfterPlayerInventoryChangeEventArgs(List<Game.PlayerInventoryChangeType> changeList, int currentTick) {
    ChangeList = changeList;
    CurrentTick = currentTick;
  }
}