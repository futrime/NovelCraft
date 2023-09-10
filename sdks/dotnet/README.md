# NovelCraft SDK for .NET

## Installation

Use the package manager [NuGet](https://www.nuget.org/) to install the SDK.

```bash
dotnet add package NovelCraft.Sdk
```

## Usage

This is a sample written in C#. Though, you can use any .NET language (C#, F#, VB.NET, etc.) to write your agent.

```csharp
using NovelCraft.Sdk;

Sdk.Initialize(args);
Sdk.Logger.Info("Hello World!");

while (true)
{
    var agent = Sdk.Agent;

    if (agent is null) {
        continue;
    }

    agent.Movement = IAgent.MovementKind.Forward;
}
```

For further information, please start with the [Sdk Class Reference](https://dotnet.sdk.novelcraft.games/classSdk.html).

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[Unlicense](choosealicense.com/licenses/unlicense/)
