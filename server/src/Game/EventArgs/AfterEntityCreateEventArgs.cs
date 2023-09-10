namespace NovelCraft.Server.Game;

public class AfterEntityCreateEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Entity> EntityList { get; }

  public AfterEntityCreateEventArgs(List<Entity> entityList, int currentTick) {
    CurrentTick = currentTick;
    EntityList = entityList;
  }
}