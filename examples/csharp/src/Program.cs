using NovelCraft.Sdk;

Sdk.Initialize(args);
Sdk.Logger.Info("Hello World!");

var lastJumpTime = DateTime.Now;

while (true) {
  var agent = Sdk.Agent;

  if (agent is null) {
    continue;
  }

  if (agent.Movement is not IAgent.MovementKind.Forward) {
    agent.Movement = IAgent.MovementKind.Forward;
  }

  if (DateTime.Now - lastJumpTime > TimeSpan.FromSeconds(1)) {
    agent.Jump();

    lastJumpTime = DateTime.Now;
  }
}
