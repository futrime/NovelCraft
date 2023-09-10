namespace NovelCraft.Server.Game;

public class AfterEntityHealEventArgs : EventArgs {
  public int CurrentTick { get; }
  public Entity Entity { get; }
  public decimal HealAmount { get; }

  public AfterEntityHealEventArgs(int currentTick, Entity entity, decimal healAmount) {
    CurrentTick = currentTick;
    Entity = entity;
    HealAmount = healAmount;
  }
}
