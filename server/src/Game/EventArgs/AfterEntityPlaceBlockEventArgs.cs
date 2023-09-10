namespace NovelCraft.Server.Game;

public class AfterEntityPlaceBlockEventArgs : EventArgs {
  public int CurrentTick { get; }
  public Entity Entity { get; }
  public Block Block { get; }

  public AfterEntityPlaceBlockEventArgs(int currentTick, Entity entity, Block block) {
    CurrentTick = currentTick;
    Entity = entity;
    Block = block;
  }
}
