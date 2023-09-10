namespace NovelCraft.Server.Game;

public class AfterEntitySpawnEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Entity> EntityList { get; }

  public AfterEntitySpawnEventArgs(List<Entity> entityList, int currentTick) {
    EntityList = entityList;
    CurrentTick = currentTick;
  }
}