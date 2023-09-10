# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.8.0] - 2023-05-28

### Changed

- Optimize performance.

- Use concurrent-capable data structures.

## [0.7.0] - 2023-05-28

### Changed

- Restrict spawn range.

## [0.6.2] - 2023-05-27

### Fixed

- Null record reference.

## [0.6.1] - 2023-05-27

### Fixed

- Negative damage.

- Duplicated records.

## [0.6.0] - 2023-05-27

### Changed

- Use new example maps.

### Fixed

- Not respawning at a new position.

## [0.5.0] - 2023-05-26

### Added

- Record when a message is received.

- Event triggered by an entity breaking a block.

- Event triggered by an entity placing a block.

- Event triggered by an entity healing.

- Food.

- Randomized spawn position.

- `give` command.

- Entity.MaxHealth property.

- `damage` command.

- Respawning.

### Changed

- Rename default player to Steve.

### Fixed

- Wrong loot table of leaves.

## [0.4.0] - 2023-05-16

### Added

- Diamond, golden and iron tool items.

- Sapling block.

- Loot table of leaves.

- Event triggered by main hand changing.

### Fixed

- Item entity falling through the ground.

## [0.3.1] - 2023-05-05

### Fixed

- Failing to run on Mac OS.

## [0.3.0] - 2023-05-05

### Changed

- Enable collision box in physics simulation.

### Fixed

- Occasionally pass through the wall.
- Exceptional two-stage falling.

## [0.2.0] - 2023-04-26

### Added

- Agent name support.
- Automatically remove .nclevel file when it has been extracted.
- Damage cause to AfterEntityHurtEventRecord.
- Time gap between two damages.
- Start holding and stop holding of using items.

### Fixed

- No longer crash when encountering invalid definition files.
- Wrong identifier of log.

## [0.1.0] - 2023-04-23

### Fixed

- Omitting the item type IDs of AfterPlayerInventoryChange message.

## [0.0.1] - 2023-04-13

- Initial release.

[unreleased]: https://github.com/NovelCraft/Server/compare/v0.8.0...HEAD
[0.8.0]: https://github.com/NovelCraft/Server/compare/v0.7.0...v0.8.0
[0.7.0]: https://github.com/NovelCraft/Server/compare/v0.6.2...v0.7.0
[0.6.2]: https://github.com/NovelCraft/Server/compare/v0.6.1...v0.6.2
[0.6.1]: https://github.com/NovelCraft/Server/compare/v0.6.0...v0.6.1
[0.6.0]: https://github.com/NovelCraft/Server/compare/v0.5.0...v0.6.0
[0.5.0]: https://github.com/NovelCraft/Server/compare/v0.4.0...v0.5.0
[0.4.0]: https://github.com/NovelCraft/Server/compare/v0.3.1...v0.4.0
[0.3.1]: https://github.com/NovelCraft/Server/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/NovelCraft/Server/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/NovelCraft/Server/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/NovelCraft/Server/compare/v0.0.1...v0.1.0
[0.0.1]: https://github.com/NovelCraft/Server/releases/tag/v0.0.1
