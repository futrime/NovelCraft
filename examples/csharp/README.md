# NovelCraft Agent Template for C#

This is a template for creating a NovelCraft agent in C#.

## Usage

1. Create a repository from this template.

2. Clone the repository to your local machine.

3. Write your agent code in `src` directory.

4. Build your agent by running `dotnet build` in the root directory.

5. Run your agent by running `dotnet run --project src/Agent.csproj` in the root directory.

    To connect to a server other than `localhost:14514` with a different token, run `dotnet run --project src/Agent.csproj -- --host <server> --port <port> --token <token>`.

6. Publish your agent by running `dotnet publish -c Release -o build --sc false -p:PublishSingleFile=true -p:DebugType=none` in the root directory.

For further instructions, refer to [the documentation](https://novelcraft.games).

## License

[Unlicense](https://choosealicense.com/licenses/unlicense/)
