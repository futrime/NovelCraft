using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Level;

public class CheckVisibility
{
    /// <summary>
    /// Check the visibility in all the sections
    /// </summary>
    /// <returns></returns>
    public static void CheckInnerVisibility(BlockCreator blockCreator)
    {
        int airId = BlockDicts.BlockDict["Air"];
        foreach (var blockSourceItem in BlockSource.SectionDict)
        {
            // Check visibility in the section
            Section nowSection = blockSourceItem.Value;

            // bx: block x , by: block y , bz: block z (relative position to nowSection)
            // Regardless of the edge which will be computed in function "CheckNeighbourVisibility"
            for (int bx = 1; bx < nowSection.Blocks.GetLength(0) - 1; bx++)
            {
                for (int by = 1; by < nowSection.Blocks.GetLength(1) - 1; by++)
                {
                    for (int bz = 1; bz < nowSection.Blocks.GetLength(2) - 1; bz++)
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[bx, by, bz];

                        if (nowSection.Blocks[bx - 1, by, bz].Id == airId ||
                            nowSection.Blocks[bx + 1, by, bz].Id == airId ||
                            nowSection.Blocks[bx, by - 1, bz].Id == airId ||
                            nowSection.Blocks[bx, by + 1, bz].Id == airId ||
                            nowSection.Blocks[bx, by, bz - 1].Id == airId ||
                            nowSection.Blocks[bx, by, bz + 1].Id == airId)
                        {
                            blockCreator.CreateBlock(nowBlock);
                        }
                    }
                }
            }

        }
    }
    /// <summary>
    /// Check the visibility between all the sections
    /// </summary>
    /// <returns></returns>
    public static void CheckNeighbourVisibility(BlockCreator blockCreator)
    {
        int airId = BlockDicts.BlockDict["Air"];

        int sectionSmallEdge = 0, sectionLargeEdge = LevelInfo.SectionLength - 1;
        foreach (var blockSourceItem in BlockSource.SectionDict)
        {
            // Check visibility on the surface of each section
            Section nowSection = blockSourceItem.Value;
            Vector3Int nowSectionPosition = blockSourceItem.Key * LevelInfo.SectionLength;

            // bx: block x , by: block y , bz: block z (relative position to nowSection)

            // X small edge in the section
            for (int by = 0; by <= sectionLargeEdge; by++)
            {
                for (int bz = 0; bz <= sectionLargeEdge; bz++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x - 1,
                        nowSectionPosition.y + by,
                        nowSectionPosition.z + bz);

                    Block xLastSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the X Last Section 
                    if (nowSection.Blocks[sectionSmallEdge + 1, by, bz].Id == airId ||  // Xnext is air

                        (xLastSectionBlock != null && xLastSectionBlock.Id == airId) || // Block in the X Last Section is air

                        (by > 0 && nowSection.Blocks[sectionSmallEdge, by - 1, bz].Id == airId) ||
                        (by < sectionLargeEdge && nowSection.Blocks[sectionSmallEdge, by + 1, bz].Id == airId) ||
                        (bz > 0 && nowSection.Blocks[sectionSmallEdge, by, bz - 1].Id == airId) ||
                        (bz < sectionLargeEdge && nowSection.Blocks[sectionSmallEdge, by, bz + 1].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[sectionSmallEdge, by, bz];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
            // X large edge in the section
            for (int by = 0; by <= sectionLargeEdge; by++)
            {
                for (int bz = 0; bz <= sectionLargeEdge; bz++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x + LevelInfo.SectionLength,
                        nowSectionPosition.y + by,
                        nowSectionPosition.z + bz);

                    Block xNextSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the X Next Section 
                    if (nowSection.Blocks[sectionLargeEdge - 1, by, bz].Id == airId ||  // Xlast is air

                        (xNextSectionBlock != null && xNextSectionBlock.Id == airId) || // block in the X Next Section is air

                        (by > 0 && nowSection.Blocks[sectionLargeEdge, by - 1, bz].Id == airId) ||
                        (by < sectionLargeEdge && nowSection.Blocks[sectionLargeEdge, by + 1, bz].Id == airId) ||
                        (bz > 0 && nowSection.Blocks[sectionLargeEdge, by, bz - 1].Id == airId) ||
                        (bz < sectionLargeEdge && nowSection.Blocks[sectionLargeEdge, by, bz + 1].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[sectionLargeEdge, by, bz];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
            //------------------------------------------------//
            // Y small edge in the section
            for (int bx = 0; bx <= sectionLargeEdge; bx++)
            {
                for (int bz = 0; bz <= sectionLargeEdge; bz++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x + bx,
                        nowSectionPosition.y - 1,
                        nowSectionPosition.z + bz);

                    Block yLastSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the Y last Section 
                    if (nowSection.Blocks[bx, sectionSmallEdge + 1, bz].Id == airId ||  // Ynext is air

                        (yLastSectionBlock != null && yLastSectionBlock.Id == airId) || // block in the Y Last Section is air

                        (bx > 0 && nowSection.Blocks[bx - 1, sectionSmallEdge, bz].Id == airId) ||
                        (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, sectionSmallEdge, bz].Id == airId) ||
                        (bz > 0 && nowSection.Blocks[bx, sectionSmallEdge, bz - 1].Id == airId) ||
                        (bz < sectionLargeEdge && nowSection.Blocks[bx, sectionSmallEdge, bz + 1].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[bx, sectionSmallEdge, bz];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
            // Y large edge in the section
            for (int bx = 0; bx <= sectionLargeEdge; bx++)
            {
                for (int bz = 0; bz <= sectionLargeEdge; bz++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x + bx,
                        nowSectionPosition.y + LevelInfo.SectionLength,
                        nowSectionPosition.z + bz);

                    Block yNextSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the Y next Section 
                    if (nowSection.Blocks[bx, sectionLargeEdge - 1, bz].Id == airId ||  // Ylast is air

                        (yNextSectionBlock != null && yNextSectionBlock.Id == airId) || // block in the X Next Section is air

                        (bx > 0 && nowSection.Blocks[bx - 1, sectionLargeEdge, bz].Id == airId) ||
                        (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, sectionLargeEdge, bz].Id == airId) ||
                        (bz > 0 && nowSection.Blocks[bx, sectionLargeEdge, bz - 1].Id == airId) ||
                        (bz < sectionLargeEdge && nowSection.Blocks[bx, sectionLargeEdge, bz + 1].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[bx, sectionLargeEdge, bz];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
            //------------------------------------------------//
            // Z small edge in the section
            for (int bx = 0; bx <= sectionLargeEdge; bx++)
            {
                for (int by = 0; by <= sectionLargeEdge; by++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x + bx,
                        nowSectionPosition.y + by,
                        nowSectionPosition.z - 1);

                    Block zLastSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the Z last Section 
                    if (nowSection.Blocks[bx, by, sectionSmallEdge + 1].Id == airId ||  // Znext is air

                        (zLastSectionBlock != null && zLastSectionBlock.Id == airId) || // Block in the Z Last Section is air

                        (bx > 0 && nowSection.Blocks[bx - 1, by, sectionSmallEdge].Id == airId) ||
                        (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, by, sectionSmallEdge].Id == airId) ||
                        (by > 0 && nowSection.Blocks[bx, by - 1, sectionSmallEdge].Id == airId) ||
                        (by < sectionLargeEdge && nowSection.Blocks[bx, by + 1, sectionSmallEdge].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[bx, by, sectionSmallEdge];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
            // Z large edge in the section
            for (int bx = 0; bx <= sectionLargeEdge; bx++)
            {
                for (int by = 0; by <= sectionLargeEdge; by++)
                {
                    Vector3Int absolutePosition = new Vector3Int(
                        nowSectionPosition.x + bx,
                        nowSectionPosition.y + by,
                        nowSectionPosition.z + LevelInfo.SectionLength);

                    Block zNextSectionBlock = BlockSource.GetBlock(absolutePosition);// Block in the Z next Section 
                    if (nowSection.Blocks[bx, by, sectionLargeEdge - 1].Id == airId ||  // Zlast is air

                        (zNextSectionBlock != null && zNextSectionBlock.Id == airId) || // Block in the Z Next Section is air

                        (bx > 0 && nowSection.Blocks[bx - 1, by, sectionLargeEdge].Id == airId) ||
                        (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, by, sectionLargeEdge].Id == airId) ||
                        (by > 0 && nowSection.Blocks[bx, by - 1, sectionLargeEdge].Id == airId) ||
                        (by < sectionLargeEdge && nowSection.Blocks[bx, by + 1, sectionLargeEdge].Id == airId))
                    {
                        // If the block is visible, create it at once
                        Block nowBlock = nowSection.Blocks[bx, by, sectionLargeEdge];

                        blockCreator.CreateBlock(nowBlock);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check visibility of other blocks around the block
    /// </summary>
    /// <param name="blockCreator"></param>
    /// <param name="block"></param>
    public static void CheckSingleBlockNeighbourVisibility(BlockCreator blockCreator, Block block)
    {
        void NeighbourBlockChange(Block neighbourBlock)
        {
            if (neighbourBlock != null)
            {
                if (block.Id == 0 && neighbourBlock.BlockObject == null)
                {
                    // Air block
                    blockCreator.CreateBlock(neighbourBlock);
                }
                else if (block.Id != 0 && neighbourBlock.BlockObject != null)
                {
                    // Destroy block
                    CheckSingleBlockVisibility(blockCreator, neighbourBlock);
                }
            }
        };

        Block XleftBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x - 1, block.Position.y, block.Position.z));
        Block XrightBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x + 1, block.Position.y, block.Position.z));
        Block YupBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y - 1, block.Position.z));
        Block YdownBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y + 1, block.Position.z));
        Block ZfrontBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y, block.Position.z - 1));
        Block ZbackBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y, block.Position.z + 1));

        NeighbourBlockChange(XleftBlock);
        NeighbourBlockChange(XrightBlock);
        NeighbourBlockChange(YupBlock);
        NeighbourBlockChange(YdownBlock);
        NeighbourBlockChange(ZfrontBlock);
        NeighbourBlockChange(ZbackBlock);
    }

    public static void CheckSingleBlockVisibility(BlockCreator blockCreator, Block block)
    {
        bool shouldCreate = false;
        void BlockChange(Block neighbourBlock)
        {
            if (neighbourBlock != null)
            {
                if (neighbourBlock.Id == 0)
                {
                    shouldCreate = true;
                    if (block.BlockObject == null)
                    {
                        // Air block
                        blockCreator.CreateBlock(block);
                    }
                }
            }
        };

        Block XleftBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x - 1, block.Position.y, block.Position.z));
        Block XrightBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x + 1, block.Position.y, block.Position.z));
        Block YupBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y - 1, block.Position.z));
        Block YdownBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y + 1, block.Position.z));
        Block ZfrontBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y, block.Position.z - 1));
        Block ZbackBlock = BlockSource.GetBlock(new Vector3Int(block.Position.x, block.Position.y, block.Position.z + 1));

        BlockChange(XleftBlock);
        BlockChange(XrightBlock);
        BlockChange(YupBlock);
        BlockChange(YdownBlock);
        BlockChange(ZfrontBlock);
        BlockChange(ZbackBlock);

        if (shouldCreate == false && block.BlockObject != null)
        {
            blockCreator.DestroyBlock(block);
        }
    }
    public static void CheckSectionVisibility(BlockCreator blockCreator, Section section)
    {
        if (section is null || blockCreator is null) return;

        for (int x = section.PositionIndex.x * 16 - 1; x <= (section.PositionIndex.x + 1) * 16; x++)
        {
            for (int y = section.PositionIndex.y * 16 - 1; y <= (section.PositionIndex.y + 1) * 16; y++)
            {
                for (int z = section.PositionIndex.z * 16 - 1; z <= (section.PositionIndex.z + 1) * 16; z++)
                {
                    Block block = BlockSource.GetBlock(new Vector3Int(x, y, z));

                    if (block is not null)
                        CheckSingleBlockVisibility(blockCreator, block);
                }
            }
        }
    }
}
