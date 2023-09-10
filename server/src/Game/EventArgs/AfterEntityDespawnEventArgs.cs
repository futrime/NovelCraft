namespace NovelCraft.Server.Game;

public class AfterEntityDespawnEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Entity> EntityList { get; }

  public AfterEntityDespawnEventArgs(List<Entity> entityList, int currentTick) {
    EntityList = entityList;
    CurrentTick = currentTick;
  }
}