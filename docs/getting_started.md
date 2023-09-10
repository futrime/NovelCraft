# Getting Started

NovelCraft is splitted into three parts: the server, the client and the agents. The server is the core of NovelCraft, it is responsible for running the game, managing the world and the players. The client is the graphical interface that the players use to interact with the game and play game records. The agents are the scripts that the players write to control their characters automatically.

In this guide, we will show you how to install and run the server and the client. We will also show you how to write a simple agent.

## Prerequisites

- [Lip](https://github.com/LipPkg/Lip): A package manager to install NovelCraft modules.

- [LipUI for NovelCraft](https://github.com/NovelCraft/LipUI): A Windows only GUI for Lip. It is not required, but it is recommended. If you use LipUI, Lip is not required.

- [.NET Runtime 7.0](https://dotnet.microsoft.com/download/dotnet/7.0): NovelCraft is written in C# and requires .NET 7.0 to run.

## Installation

We recommend you to install NovelCraft using Lip, since it is the easiest way to install and update NovelCraft. If you want to install NovelCraft manually, you can download the latest release from the releases pages of each module.

In this guide, we assume that you have installed Lip and added it to your PATH environment variable. If you do not want to add Lip to your PATH, you can use the full path to the Lip executable instead of `lip` in the following commands.

1. Create a directory for NovelCraft. We recommend not using an existing directory with files.

2. Set the `LIP_REGISTRY` environment variable to `https://registry.novelcraft.games`. If you are using LipUI for NovelCraft, you can skip this step.

    In Command Prompt of Windows, you can use the following command to set the environment variable:

    ```bash
    set LIP_REGISTRY=https://registry.novelcraft.games
    ```

    In PowerShell, you can use the following command to set the environment variable:

    ```powershell
    $env:LIP_REGISTRY = "https://registry.novelcraft.games"
    ```

    In Unix-like systems, you can use the following command to set the environment variable:

    ```bash
    export LIP_REGISTRY=https://registry.novelcraft.games
    ```

3. Install the server and the client. Note that you should run this command in the directory you created in step 1.

    ```bash
    lip install server client
    ```

4. Now you have installed the server and the client.

## Run the Server

Before running the server, you need to modify the configuration file. The configuration file is located at `config.json`. You can use any text editor to modify the configuration file. The items in the configuration file are shown below:

- `server_port`: The port that the server listens on. You can change it to any port you want.

- `level_name`: The name of the level that the server loads. You can change it to any name you want. The level files are located at `worlds/<level_name>/`. If the level does not exist, the server will extract `worlds/<level_name>.nclevel`. If the level file does not exist, the server will create a new level.

- `ticks_per_second`: The number of ticks per second. The server will run `ticks_per_second` ticks per second. You can change it to any number you want. Note that if you change it to a very large number, the server may not be able to keep up with the ticks.

- `save_level`: Whether to save the level when the server stops. If it is `true`, the server will save the level when it stops. If it is `false`, the server will not save the level when it stops.

- `save_record`: Whether to save the game records when the server stops. If it is `true`, the server will save the game records during the game. If it is `false`, the server will not save the game records during the game.

- `player_spawn_max_y`: The maximum y coordinate of the player spawn. The server will generate a random spawn point for the player. The y coordinate of the spawn point will be less than or equal to `player_spawn_max_y`.

- `waiting_time`: The waiting time of the server. The server will wait for `waiting_time` seconds before starting the game. You can change it to any number you want.

- `max_tick`: The maximum number of ticks of the game. The server will stop the game after `max_tick` ticks. You can change it to any number you want. If you want the game to run forever, you can set it to `null`.

To let your agent or client connect to the server, you should add some tokens in `whitelist.json`. Each item of the array in `whitelist.json` is a token. You can add as many tokens as you want.

Note that the default token of the client and the agents is "" (an empty string). If you want to use the client or the agents, you should add an empty string to `whitelist.json`.

Now you can run the server. You can use the following command to run the server:

On Unix-like systems:

```bash
chmod +x Server
./Server
```

On Windows:

```powershell
.\Server.exe
```

## Run the Client

The client does not need any configuration. You can directly run the `Client` executable (or `Client.exe` on Windows).

## Import a Level

It is easy to import a level to NovelCraft. You can just put the `.nclevel` file in the `worlds` directory, or put a folder containing `level.dat` in the `worlds` directory. The level will be automatically imported.

## Create an Agent

We provide various agent templates in different programming languages. You can use the agent templates to write your own agents. There are two ways to initialize an agent: using the GitHub template repository or get with Lip. Here is a list of supported agent templates:

- [NovelCraft Agent Template for C#](https://github.com/NovelCraft/AgentTemplate-CSharp)

- [NovelCraft Agent Template for Python](https://github.com/NovelCraft/AgentTemplate-Python)

In this guide, we will show you how to create an agent written in C#.

1. Go to the [agent template repository](https://github.com/NovelCraft/AgentTemplate-CSharp).

2. Click the `Use this template` button.

3. Enter the name of the repository and click the `Create repository from template` button.

4. Clone the repository to your computer.

5. Open the repository in your favorite IDE.

6. Modify the `src/Program.cs` file to write your own agent. You can also modify any other files.

7. Build your agent.

## Run a Game with an Agent

1. Run the server.

2. Run the agent. You should pass the address of the server, the port of the server, and the token of the server to the agent. For example, if the server is running on `1.14.5.14` at port `14514` and the token is `token`, you can run the agent with the following command:

    ```bash
    ./Agent --token token --host 1.14.5.14 --port 14514
    ```

3. Then you will see a message on the server console indicating that the agent has connected to the server.

4. Now the game has started. You can wait for the game to end, or you can stop the server to end the game. To stop the server, type `stop` in the server console and press Enter.

## Playback

The client supports playback. You can use the client to playback a game. To playback a game, you should just run the client and select the world to playback. The client will automatically playback the game.

## Play the Game with the Client

You can use the client to play the game. To play the game, you should just run the client and select a remote server to connect to. Or starts a local server.

## Next Steps

You can see tutorials of other topics in the **tutorials** section.

If you are encountering any problems, you can see [the **FAQ** section](/faq.md). If you cannot find the answer to your question, you can open an issue under the repository of the module you experienced the problem with.
