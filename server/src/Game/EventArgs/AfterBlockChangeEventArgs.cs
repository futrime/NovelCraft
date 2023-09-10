namespace NovelCraft.Server.Game;

public class AfterBlockChangeEventArgs : EventArgs {
  public int CurrentTick { get; }
  public Dictionary<Position<int>, int> BlockChangeList { get; }

  public AfterBlockChangeEventArgs(Dictionary<Position<int>, int> blockChangeList, int currentTick) {
    BlockChangeList = blockChangeList;
    CurrentTick = currentTick;
  }
}