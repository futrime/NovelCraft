namespace NovelCraft.Server.Game;

public struct EntityDamageCause {
  public enum KindType {
    None,
    EntityAttack,
    Falling
  }

  public Entity? Attacker { get; }
  public KindType Kind { get; }

  public EntityDamageCause(KindType kind, Entity? attacker = null) {
    Attacker = attacker;
    Kind = kind;
  }
}
