namespace NovelCraft.Server.Game;

public class AfterEntityBreakBlockEventArgs : EventArgs {
  public int CurrentTick { get; }
  public Entity Entity { get; }
  public Block Block { get; }

  public AfterEntityBreakBlockEventArgs(int currentTick, Entity entity, Block block) {
    CurrentTick = currentTick;
    Entity = entity;
    Block = block;
  }
}
