namespace NovelCraft.Server.Game;

public class AfterEntityOrientationChangeEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Entity> EntityList { get; }

  public AfterEntityOrientationChangeEventArgs(List<Entity> entityList, int currentTick) {
    EntityList = entityList;
    CurrentTick = currentTick;
  }
}