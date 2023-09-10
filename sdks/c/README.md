# NovelCraft SDK for C/C++

NovelCraft SDK for C/C++ is a C/C++ library for communicating with NovelCraft Servers and accessing NovelCraft game data.

## Installation

Add this repository as a submodule to your project:

```bash
git submodule add https://github.com/NovelCraft/SDK-C.git sdk
git submodule update --init --recursive
```

Add the following to your CMakeLists.txt:

```cmake
add_subdirectory(sdk)

target_link_libraries(${PROJECT_NAME} PRIVATE ncsdk)
```

## Usage

```c
#include <sdk.h>
#include <time.h>

int main(int argc, char* argv[]) {
  // Initialize the SDK
  ncsdk_Initialize(argc, argv);
  ncsdk_Logger_Info(ncsdk_GetLogger(), "Hello, world!");

  while (true) {
    ncsdk_Refresh();

    ncsdk_Agent* agent = ncsdk_GetAgent();

    if (agent == NULL) {
      continue;
    }

    ncsdk_Agent_SetMovement(agent, ncsdk_Agent_MovementKind_Forward);
  }

  // Finalize the SDK
  ncsdk_Finalize();
  ncsdk_Logger_Info(ncsdk_GetLogger(), "Goodbye, world!");

  return 0;
}
```

For further information, please start with the [sdk.h File Reference](https://c.sdk.novelcraft.games/sdk_8h.html).

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[Unlicense](choosealicense.com/licenses/unlicense/)
