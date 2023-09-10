# Minecraft Level Exporter

Minecraft Level Exporter is a tool to export and convert a Minecraft world to a NovelCraft level.

## Prerequisites

- [LiteLoaderBDS](https://docs.litebds.com): A Minecraft Bedrock Dedicated Server with plugins support.

- [Lip](https://docs.lippkg.com): A package manager.

## Installation

Assume that you have installed LiteLoaderBDS and Lip. Run the following commands under the folder of LiteLoaderBDS to install this tool.

If you are using Command Prompt, run the following commands:

```bat
set LIP_REGISTRY=https://registry.novelcraft.games
lip install exporter
```

If you are using PowerShell, run the following commands:

```powershell
$env:LIP_REGISTRY = "https://registry.novelcraft.games"
lip install exporter
```

## Usage

1. Run the LiteLoaderBDS server.

2. Join the server with Minecraft.

3. Wait for the world to load.

4. Run `export x0 y0 z0 x1 y1 z1` in the console. The coordinates are the start and end coordinates of the exported area.

5. Run `lip x convert .\plugins\level_exporter\level_data.json .\dict.json mylevel.nclevel` to convert the level data to a NovelCraft level.

6. Get the exported level in `mylevel.nclevel`.

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[GPL-3.0](https://choosealicense.com/licenses/gpl-3.0/)
