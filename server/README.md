# NovelCraft Server

NovelCraft Servers allow players to play online or via a local area network with other people. It also allow agents to play with each other. Internally, the game runs a server for local play so that it is possible to play with other people on the same computer.

## Prerequisites

- [.NET Runtime 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Installation

Since NovelCraft Server is published in portable form, you can run it from anywhere. Just download the latest version from the [releases page](https://github.com/NovelCraft/Server/releases) and extract it to a folder. Then, run the executable file.

## Usage

Before running the server, you may want to change the configuration. There are two configuration files: `config.json` and `whitelist.json`. The former is used to configure the server, while the latter is used to manage the whitelist.

If you would like to play another world, you can put the .nclevel file under the `worlds` folder. Then, change the `level_name` field in `config.json` to the name of the world file.

For further information, please refer to the [documentation](https://novelcraft.games).

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[LGPL-3.0](https://choosealicense.com/licenses/lgpl-3.0/)
