namespace NovelCraft.Server.Game;

public partial class Entity {
  #region Nested classes, enums, delegates and events
  public class AfterDespawnEventArgs : EventArgs {
    public Entity Entity { get; }

    public AfterDespawnEventArgs(Entity entity) {
      Entity = entity;
    }
  }

  public class AfterHurtEventArgs : EventArgs {
    public Entity Victim { get; }
    public decimal Damage { get; }
    public EntityDamageCause DamageCause { get; }

    public AfterHurtEventArgs(Entity victim, decimal damage, EntityDamageCause damageCause) {
      Victim = victim;
      Damage = damage;
      DamageCause = damageCause;
    }
  }

  public class AfterOrientationChangeEventArgs : EventArgs {
    public Entity Entity { get; }

    public AfterOrientationChangeEventArgs(Entity entity) {
      Entity = entity;
    }
  }

  public class AfterPositionChangeEventArgs : EventArgs {
    public Entity Entity { get; }

    public AfterPositionChangeEventArgs(Entity entity) {
      Entity = entity;
    }
  }

  public class AfterSpawnEventArgs : EventArgs {
    public Entity Entity { get; }

    public AfterSpawnEventArgs(Entity entity) {
      Entity = entity;
    }
  }

  public class TryAttackEventArgs : EventArgs {
    public Entity Attacker { get; }
    public InteractionKind Kind { get; }
    public int Tick { get; }

    public TryAttackEventArgs(Entity attacker, InteractionKind kind, int tick) {
      Attacker = attacker;
      Kind = kind;
      Tick = tick;
    }
  }

  public class TryUseEventArgs : EventArgs {
    public Entity User { get; }
    public InteractionKind Kind { get; }

    public TryUseEventArgs(Entity user, InteractionKind kind) {
      User = user;
      Kind = kind;
    }
  }

  public event EventHandler<AfterDespawnEventArgs>? AfterDespawnEvent;
  public event EventHandler<AfterHurtEventArgs>? AfterHurtEvent;
  public event EventHandler<AfterOrientationChangeEventArgs>? AfterOrientationChangeEvent;
  public event EventHandler<AfterPositionChangeEventArgs>? AfterPositionChangeEvent;
  public event EventHandler<AfterSpawnEventArgs>? AfterSpawnEvent;
  public event EventHandler<TryAttackEventArgs>? TryAttackEvent;
  public event EventHandler<TryUseEventArgs>? TryUseEvent;
  #endregion
}
