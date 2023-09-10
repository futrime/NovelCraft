namespace NovelCraft.Server.Game;

public class AfterEntityRemoveEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<int> UniqueIdList { get; }

  public AfterEntityRemoveEventArgs(List<int> uniqueIdList, int currentTick) {
    UniqueIdList = uniqueIdList;
    CurrentTick = currentTick;
  }
}