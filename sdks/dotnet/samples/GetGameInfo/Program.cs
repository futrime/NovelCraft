using NovelCraft.Sdk;

Console.Write("Enter the host: ");
string host = Console.ReadLine()!;

Console.Write("Enter the port: ");
string port = Console.ReadLine()!;

Console.Write("Enter the token: ");
string token = Console.ReadLine()!;

Sdk.Initialize(new string[] {
  "--token", token,
  "--host", host,
  "--port", port,
});

while (true) {
  if (Sdk.Agent is not null) {
    Sdk.Logger.Info(
      $"Inventory content: {string.Join(", ", Enumerable.Range(0, Sdk.Agent.Inventory.Size).Select(i => Sdk.Agent.Inventory[i]?.TypeId ?? 0))}");
    Sdk.Logger.Info($"Inventory hot bar size: {Sdk.Agent.Inventory.HotBarSize}");
    Sdk.Logger.Info($"Inventory main hand slot: {Sdk.Agent.Inventory.MainHandSlot}");
    Sdk.Logger.Info($"Inventory size: {Sdk.Agent.Inventory.Size}");

    Sdk.Logger.Info($"Movement: {Sdk.Agent.Movement}");
    Sdk.Logger.Info($"Token: {Sdk.Agent.Token}");
  }

  if (Sdk.Blocks is not null) {
    Sdk.Logger.Info(
      $"Some blocks: {Sdk.Blocks[new Position<int>(0, 0, 0)]?.TypeId}, {Sdk.Blocks[new Position<int>(0, 1, 0)]?.TypeId}");
  }

  if (Sdk.Entities is not null) {
    string entities = string.Join(", ", Sdk.Entities.GetAllEntities().Select(e => e.UniqueId));
    Sdk.Logger.Info($"All entities: {entities}");
  }

  if (Sdk.Tick is not null) {
    Sdk.Logger.Info($"Tick: {Sdk.Tick}");
  }

  if (Sdk.TicksPerSecond is not null) {
    Sdk.Logger.Info($"Ticks per second: {Sdk.TicksPerSecond:0.00}");
  }

  if (Sdk.Latency is not null) {
    Sdk.Logger.Info($"Latency: {Sdk.Latency?.TotalMilliseconds:0.00}ms");
  }

  if (Sdk.Client is not null) {
    Sdk.Logger.Info($"Bandwidth: {Sdk.Client.BandWidth:0.00} Mbps");
  }

  // Sleep for 1 second
  Thread.Sleep(1000);
}

struct Position<T> : IPosition<T> {
  public T X { get; set; }
  public T Y { get; set; }
  public T Z { get; set; }

  public Position(T x, T y, T z) {
    X = x;
    Y = y;
    Z = z;
  }
}