# NovelCraft SDK for Python

NovelCraft SDK for Python is a Python library for communicating with NovelCraft Servers and accessing NovelCraft game data.

## Installation

Use the package manager [pip](https://pip.pypa.io/en/stable/) to install.

```bash
pip install novelcraft.sdk
```

## Usage

```python
import asyncio

import novelcraft.sdk as sdk


async def main():
    await sdk.initialize()

    # Do something

    await sdk.finalize()

if __name__ == '__main__':
    asyncio.run(main())
```

For more information, please start with _placeholder_.

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[Unlicense](choosealicense.com/licenses/unlicense/)
