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
  if (Sdk.Latency is not null) {
    Sdk.Logger.Info($"Latency: {Sdk.Latency?.TotalMilliseconds}ms");
  }
  
  // Sleep for 1 second
  Thread.Sleep(1000);
}