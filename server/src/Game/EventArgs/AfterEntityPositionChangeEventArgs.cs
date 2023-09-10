namespace NovelCraft.Server.Game;

public class AfterEntityPositionChangeEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Entity> EntityList { get; }

  public AfterEntityPositionChangeEventArgs(List<Entity> entityList, int currentTick) {
    EntityList = entityList;
    CurrentTick = currentTick;
  }
}