namespace NovelCraft.Server.Game;

public partial class Game {
  public event EventHandler<AfterBlockChangeEventArgs>? AfterBlockChangeEvent;
  public event EventHandler<AfterEntityAttackEventArgs>? AfterEntityAttackEvent;
  public event EventHandler<AfterEntityBreakBlockEventArgs>? AfterEntityBreakBlockEvent;
  public event EventHandler<AfterEntityCreateEventArgs>? AfterEntityCreateEvent;
  public event EventHandler<AfterEntityDespawnEventArgs>? AfterEntityDespawnEvent;
  public event EventHandler<AfterEntityHealEventArgs>? AfterEntityHealEvent;
  public event EventHandler<AfterEntityHurtEventArgs>? AfterEntityHurtEvent;
  public event EventHandler<AfterEntityOrientationChangeEventArgs>? AfterEntityOrientationChangeEvent;
  public event EventHandler<AfterEntityPlaceBlockEventArgs>? AfterEntityPlaceBlockEvent;
  public event EventHandler<AfterEntityPositionChangeEventArgs>? AfterEntityPositionChangeEvent;
  public event EventHandler<AfterEntityRemoveEventArgs>? AfterEntityRemoveEvent;
  public event EventHandler<AfterEntitySpawnEventArgs>? AfterEntitySpawnEvent;
  public event EventHandler<AfterGameRunEventArgs>? AfterGameStartEvent;
  public event EventHandler<AfterGameTickEventArgs>? AfterGameTickEvent;
  public event EventHandler<AfterPlayerInventoryChangeEventArgs>? AfterPlayerInventoryChangeEvent;
  public event EventHandler<AfterPlayerSwitchMainHandArgs>? AfterPlayerSwitchMainHandEvent;
}
