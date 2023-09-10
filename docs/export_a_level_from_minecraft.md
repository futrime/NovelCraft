# Export a Level from Minecraft

In this tutorial, you will learn how to export a level from Minecraft and import it into NovelCraft.

## Prerequisites

- **Lip** You should install Lip in advance. For more information, refer to [Lip's documentation](https://docs.lippkg.com).

    - **LipUI** LipUI is a GUI for Lip, which is recommended.

- **Minecraft Bedrock Edition** You should have a Minecraft Bedrock Edition client in advance. You can download it from [Minecraft.net](https://www.minecraft.net/en-us/download). However, you may have to purchase a license to play it.

## Setting up environment (Lip)

Create a folder (e.g. `D:\BDS`) and open command prompt under the folder. You may hold Shift and right click on the folder and select "Open command window here" to open command prompt.

Run the following command to switch to NovelCraft dedicated Lip registry.

```bat
set LIP_REGISTRY=https://registry.novelcraft.games
```

If you are using PowerShell, run the following command instead.

```powershell
$env:LIP_REGISTRY = "https://registry.novelcraft.games"
```

Run the following command to install Minecraft Level Exporter and all its dependencies:

```shell
lip install exporter
```

Start the BDS by running bedrock_server_mod.exe.

## Setting up environment (LipUI)

If you prefer a GUI, you can use LipUI instead. Download the NovelCraft-adapted edition from <https://github.com/NovelCraft/LipUI/releases>

Then you can feel free to use LipUI to install Minecraft Level Exporter and all its dependencies.

## Use a custom level (optional)

You should close the BDS before you do this.

If you would like to use a custom level exported from Minecraft Bedrock Edition (with extension name .mcworld), you can unzip everything in the .mcworld file to a folder with the same name as the level name (called level folder) under the `worlds` folder under the BDS folder. For example, `D:\BDS\worlds\Bedrock level`.

Check if the content of levelname.txt file under the level folder, which is the level name, is the same as the level folder name. If not, change the level folder name to the same as the level name.

Start the BDS by running bedrock_server_mod.exe.

## Export a level from Minecraft

If you are running the BDS on your computer, you may need to disable the net isolation with the following command:

```shell
CheckNetIsolation.exe LoopbackExempt -a -p=S-1-15-2-1958404141-86561845-1752920682-3514627264-368642714-62675701-733520436
```

Connect to the BDS with your Minecraft client. Note that DO NOT use `localhost` as the server address. Use the IP address of your computer instead. For example, `127.0.0.1`.

Wait till the world is loaded. If you would like to export a certain area, you should move your character to the area you want to export and wait till the world is loaded.

Run the following command **in the server console** to export the level:

```shell
export <start position> <end position>
```

For example:

```shell
export 0 0 0 100 100 100
```

Note that the end position is inclusive.

## Convert the level to NovelCraft format

Run the following command to convert the level to NovelCraft format:

```bat
lip x convert .\plugins\level_exporter\level_data.json .\dict.json mylevel.nclevel
```

The NovelCraft level will be generated in the BDS folder. For example, `D:\BDS\mylevel.nclevel`.

## Load the level in NovelCraft

Put the level file in the `worlds` folder under the NovelCraft folder. For example, `D:\NovelCraft\worlds\mylevel.nclevel`. Make sure that there is no folder with the same name as the level file name. For example, `D:\NovelCraft\worlds\mylevel`. If so, delete it, rename it or rename the .nclevel file.

Start NovelCraft and load the level.
