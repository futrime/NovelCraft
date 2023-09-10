#include <gtest/gtest.h>
#include <internal/block.h>
#include <internal/section.h>

#include <random>

TEST(SectionTest, Tests) {
  int block_id_array[16][16][16];
  std::random_device rd;
  std::mt19937 gen(rd());
  std::uniform_int_distribution<> dis(0, 255);
  for (int x = 0; x < 16; ++x) {
    for (int y = 0; y < 16; ++y) {
      for (int z = 0; z < 16; ++z) {
        block_id_array[x][y][z] = dis(gen);
      }
    }
  }

  ncsdk_Position(int) position = {dis(gen), dis(gen), dis(gen)};

  ncsdk_Section* section = ncsdk_Section_New(&position, block_id_array);
  for (int x = 0; x < 16; ++x) {
    for (int y = 0; y < 16; ++y) {
      for (int z = 0; z < 16; ++z) {
        ncsdk_Position(int) relative_position = {x, y, z};
        const ncsdk_Block* block =
            ncsdk_Section_GetBlock(section, &relative_position);
        EXPECT_EQ(ncsdk_Block_GetTypeId(block), block_id_array[x][y][z]);
      }
    }
  }

  EXPECT_EQ(ncsdk_Section_GetPosition(section)->x, position.x);
  EXPECT_EQ(ncsdk_Section_GetPosition(section)->y, position.y);
  EXPECT_EQ(ncsdk_Section_GetPosition(section)->z, position.z);

  // Randomly set a block.
  for (int i = 0; i < 10000; ++i) {
    ncsdk_Position(int)
        relative_position = {dis(gen) % 16, dis(gen) % 16, dis(gen) % 16};
    ncsdk_Block* block = ncsdk_Block_New(dis(gen), &relative_position);

    block_id_array[relative_position.x][relative_position.y]
                  [relative_position.z] = ncsdk_Block_GetTypeId(block);

    ncsdk_Section_SetBlock(section, &relative_position, block);
    EXPECT_EQ(ncsdk_Block_GetTypeId(block),
              ncsdk_Block_GetTypeId(
                  ncsdk_Section_GetBlock(section, &relative_position)));
    ncsdk_Block_Delete(block);
  }

  // Check that the block was set.
  for (int x = 0; x < 16; ++x) {
    for (int y = 0; y < 16; ++y) {
      for (int z = 0; z < 16; ++z) {
        ncsdk_Position(int) relative_position = {x, y, z};
        const ncsdk_Block* block =
            ncsdk_Section_GetBlock(section, &relative_position);
        EXPECT_EQ(ncsdk_Block_GetTypeId(block), block_id_array[x][y][z]);
      }
    }
  }

  ncsdk_Section_Delete(section);
}

TEST(SectionTest, HandlesOutOfRange) {
  int block_id_array[16][16][16] = {0};
  ncsdk_Position(int) position = {0, 0, 0};

  ncsdk_Section* section = ncsdk_Section_New(&position, block_id_array);

  ncsdk_Position(int) relative_position = {0, 0, 0};
  ncsdk_Block* block = ncsdk_Block_New(0, &relative_position);
  ncsdk_Section_SetBlock(section, &relative_position, block);

  relative_position = {0, 0, 16};
  block = ncsdk_Block_New(0, &relative_position);
  ncsdk_Section_SetBlock(section, &relative_position, block);
  const ncsdk_Block* block_got = ncsdk_Section_GetBlock(section, &relative_position);
  EXPECT_EQ(block_got, nullptr);

  ncsdk_Section_Delete(section);
}
