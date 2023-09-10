using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using System.IO.Compression;
using Unity.VisualScripting.FullSerializer;
/// <summary>
/// This script must be bound in a button
/// </summary>
public class LevelLegacy : MonoBehaviour
{
    public class LevelInfoLegacyLegacy
    {
        public const int SectionLength = 16;
        public const int BlockNumInSections = SectionLength * SectionLength * SectionLength;
        public const int XSectionNum = 1;
        public const int YSectionNum = 8;
        public const int ZSectionNum = 1;
        /// <summary>
        /// "Sections" contains all the sections (3D array)
        /// The length of Section.Blocks array must be equal to 'BlockNumInSections'
        /// </summary>
        public Section[,,] Sections;
        public LevelInfoLegacyLegacy(Section[,,] sections)
        {
            this.Sections = sections;
        }
    }
    private BlockCreator _blockCreator;
    private LevelInfoLegacyLegacy _levelInfo;
    private Upload _upload = new() { };
    private Upload.OpenFileName _levelFile = new() { };
    /// <summary>
    /// Get the block name using block id
    /// </summary>
    public string[] BlockNameArray;

    /// <summary>
    /// The block dict <string name, int id>
    /// </summary>
    public Dictionary<string, int> BlockDict;
    private void Start()
    {
        // Initialize the Sections
        this._levelInfo = new LevelInfoLegacyLegacy(new Section[LevelInfoLegacyLegacy.XSectionNum, LevelInfoLegacyLegacy.YSectionNum, LevelInfoLegacyLegacy.ZSectionNum]);
        // Initialize the BlockCreator
        this._blockCreator = GameObject.Find("BlockCreator").GetComponent<BlockCreator>();
        // Initialize the Dict and BlockNameArray
        this.BlockDict = JsonUtility.ParseBlockDictJson("Json/Dict");
        this.BlockNameArray = DictUtility.BlockDictParser(this.BlockDict);
        // Get json file
        var fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();
        // Check if the file is Level json
        this._levelFile = fileLoaded.File;
        if (fileLoaded.Type == FileLoaded.FileType.Level)
        {
            Run();
        }
    }
    private void Run()
    {
        // Check
        if (this._levelFile == null)
        {
            Debug.Log("Loading file error!");
            return;
        }
        LoadBlockData();
        CheckVisibility();
    }
    public void LoadBlockData()
    {
        // Read the json file and process the replay
        JObject jsonObject = (JsonUtility.UnzipLevel(this._levelFile.File));
        // Deal with Sections: array
        JArray sections = (JArray)jsonObject["sections"];
        Debug.Log(sections.Count);
        for (int i = 0; i < sections.Count; i++)
        {
            // Compute the absolute position of now section / 16
            //int sectionX = i / (LevelInfoLegacyLegacy.ZSectionNum * LevelInfoLegacyLegacy.YSectionNum);
            //int sectionY = (i - sectionX * (LevelInfoLegacyLegacy.YSectionNum * LevelInfoLegacyLegacy.ZSectionNum)) / LevelInfoLegacyLegacy.ZSectionNum;
            //int sectionZ = i % LevelInfoLegacyLegacy.ZSectionNum;
            int sectionX = 0;
            int sectionY = i;
            int sectionZ = 0;
            // All blocks in one section
            Section section = new(new Vector3Int(sectionX, sectionY, sectionZ));

            // jsonSection: array<int blockID>
            JArray jsonSection = (JArray)(sections[i]["blocks"]);
            if (jsonSection.Count != LevelInfoLegacyLegacy.BlockNumInSections)
            {
                throw new System.Exception($"The length per section is not {LevelInfoLegacyLegacy.BlockNumInSections}");
            }

            for (int j = 0; j < jsonSection.Count; j++)
            {
                // Compute relative position <The blocks in the section which can be accessed by `blocks[x*256+y*16+z]>
                int x = j / 256;
                int y = j / 16 - x * 16;
                int z = j % 16;
                // Initialize the block
                section.Blocks[x, y, z] = new Block();
                // BlockID
                section.Blocks[x, y, z].Id = short.Parse(jsonSection[j].ToString());
                // Add name according to _blockNameArray
                try
                {
                    section.Blocks[x, y, z].Name = BlockNameArray[section.Blocks[x, y, z].Id];
                }
                catch
                {
                    Debug.Log(BlockNameArray);
                    Debug.Log(section.Blocks[x, y, z].Id);
                    section.Blocks[x, y, z].Name = BlockNameArray[0];
                    section.Blocks[x, y, z].Id = 0;
                }
                // Compute absolute position
                section.Blocks[x, y, z].Position = new Vector3Int(sectionX * LevelInfoLegacyLegacy.SectionLength + x, sectionY * LevelInfoLegacyLegacy.SectionLength + y, sectionZ * LevelInfoLegacyLegacy.SectionLength + z);
            }
            //try
            //{
            this._levelInfo.Sections[sectionX, sectionY, sectionZ] = section;
            //}
            //catch
            //{
            //    Debug.Log(i);
            //    Debug.Log(new Vector3Int(sectionX, sectionY, sectionZ));
            //}

        }
    }
    public void CheckVisibility()
    {
        this.CheckInnerVisibility();
        this.CheckNeighbourVisibility();
    }
    /// <summary>
    /// Get the section index by using x,y,z index
    /// </summary>
    /// <param name="xIndex"> The x index of section </param>
    /// <param name="yIndex"> The y index of section </param>
    /// <param name="zIndex"> The z index of section </param>
    /// <returns></returns>
    private int GetSectionIndex(int xIndex, int yIndex, int zIndex)
    {
        return xIndex * LevelInfoLegacyLegacy.YSectionNum * LevelInfoLegacyLegacy.ZSectionNum + yIndex * LevelInfoLegacyLegacy.ZSectionNum + zIndex;
    }
    /// <summary>
    /// Check the visibility in all the sections
    /// </summary>
    /// <returns></returns>
    private void CheckInnerVisibility()
    {
        int airId = this.BlockDict["Air"];
        // sx: section x , sy: section y , sz: section z
        for (int sx = 0; sx < this._levelInfo.Sections.GetLength(0); sx++)
        {
            for (int sy = 0; sy < this._levelInfo.Sections.GetLength(1); sy++)
            {
                for (int sz = 0; sz < this._levelInfo.Sections.GetLength(2); sz++)
                {
                    // Check visibility in the section
                    Section nowSection = this._levelInfo.Sections[sx, sy, sz];
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
                                    this._blockCreator.CreateBlock(nowBlock);
                                }
                            }
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
    private void CheckNeighbourVisibility()
    {
        int airId = this.BlockDict["Air"];
        // We can solve this problem by dividing it into many parallel planes which are the surface of each section
        int sectionSmallEdge = 0, sectionLargeEdge = LevelInfoLegacyLegacy.SectionLength - 1;

        int sectionsLenX = this._levelInfo.Sections.GetLength(0);
        int sectionsLenY = this._levelInfo.Sections.GetLength(1);
        int sectionsLenZ = this._levelInfo.Sections.GetLength(2);

        // sx: section x , sy: section y , sz: section z
        for (int sx = 0; sx < sectionsLenX; sx++)
        {
            for (int sy = 0; sy < sectionsLenY; sy++)
            {
                for (int sz = 0; sz < sectionsLenZ; sz++)
                {
                    // Check visibility on the surface of each section
                    Section nowSection = _levelInfo.Sections[sx, sy, sz];
                    // Neighbor sections
                    Section xLastSection = sx > 0 ? _levelInfo.Sections[sx - 1, sy, sz] : null;
                    Section xNextSection = sx < sectionsLenX - 1 ? _levelInfo.Sections[sx + 1, sy, sz] : null;
                    Section yLastSection = sy > 0 ? _levelInfo.Sections[sx, sy - 1, sz] : null;
                    Section yNextSection = sy < sectionsLenY - 1 ? _levelInfo.Sections[sx, sy + 1, sz] : null;
                    Section zLastSection = sz > 0 ? _levelInfo.Sections[sx, sy, sz - 1] : null;
                    Section zNextSection = sz < sectionsLenZ - 1 ? _levelInfo.Sections[sx, sy, sz + 1] : null;

                    // bx: block x , by: block y , bz: block z (relative position to nowSection)
                    // X small edge in the section
                    for (int by = 0; by <= sectionLargeEdge; by++)
                    {
                        for (int bz = 0; bz <= sectionLargeEdge; bz++)
                        {
                            if (sx == 0 || // on the edge of whole map
                                nowSection.Blocks[sectionSmallEdge + 1, by, bz].Id == airId ||  // Xnext is air

                                (xLastSection != null && xLastSection.Blocks[sectionLargeEdge, by, bz].Id == airId) || // block in the X Last Section is air

                                (by > 0 && nowSection.Blocks[sectionSmallEdge, by - 1, bz].Id == airId) ||
                                (by < sectionLargeEdge && nowSection.Blocks[sectionSmallEdge, by + 1, bz].Id == airId) ||
                                (bz > 0 && nowSection.Blocks[sectionSmallEdge, by, bz - 1].Id == airId) ||
                                (bz < sectionLargeEdge && nowSection.Blocks[sectionSmallEdge, by, bz + 1].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[sectionSmallEdge, by, bz];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                    // X large edge in the section
                    for (int by = 0; by <= sectionLargeEdge; by++)
                    {
                        for (int bz = 0; bz <= sectionLargeEdge; bz++)
                        {
                            if (sx == sectionsLenX - 1 || // On the edge of whole map
                                nowSection.Blocks[sectionLargeEdge - 1, by, bz].Id == airId ||  // Xlast is air

                                (xNextSection != null && xNextSection.Blocks[sectionSmallEdge, by, bz].Id == airId) || // block in the X Next Section is air

                                (by > 0 && nowSection.Blocks[sectionLargeEdge, by - 1, bz].Id == airId) ||
                                (by < sectionLargeEdge && nowSection.Blocks[sectionLargeEdge, by + 1, bz].Id == airId) ||
                                (bz > 0 && nowSection.Blocks[sectionLargeEdge, by, bz - 1].Id == airId) ||
                                (bz < sectionLargeEdge && nowSection.Blocks[sectionLargeEdge, by, bz + 1].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[sectionLargeEdge, by, bz];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                    //------------------------------------------------//
                    // Y small edge in the section
                    for (int bx = 0; bx <= sectionLargeEdge; bx++)
                    {
                        for (int bz = 0; bz <= sectionLargeEdge; bz++)
                        {
                            if (sy == 0 || // on the edge of whole map
                                nowSection.Blocks[bx, sectionSmallEdge + 1, bz].Id == airId ||  // Ynext is air

                                (yLastSection != null && yLastSection.Blocks[bx, sectionLargeEdge, bz].Id == airId) || // block in the X Last Section is air

                                (bx > 0 && nowSection.Blocks[bx - 1, sectionSmallEdge, bz].Id == airId) ||
                                (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, sectionSmallEdge, bz].Id == airId) ||
                                (bz > 0 && nowSection.Blocks[bx, sectionSmallEdge, bz - 1].Id == airId) ||
                                (bz < sectionLargeEdge && nowSection.Blocks[bx, sectionSmallEdge, bz + 1].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[bx, sectionSmallEdge, bz];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                    // Y large edge in the section
                    for (int bx = 0; bx <= sectionLargeEdge; bx++)
                    {
                        for (int bz = 0; bz <= sectionLargeEdge; bz++)
                        {
                            if (sy == sectionsLenY - 1 || // On the edge of whole map
                                nowSection.Blocks[bx, sectionLargeEdge - 1, bz].Id == airId ||  // Ylast is air

                                (xNextSection != null && yNextSection.Blocks[bx, sectionSmallEdge, bz].Id == airId) || // block in the X Next Section is air

                                (bx > 0 && nowSection.Blocks[bx - 1, sectionLargeEdge, bz].Id == airId) ||
                                (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, sectionLargeEdge, bz].Id == airId) ||
                                (bz > 0 && nowSection.Blocks[bx, sectionLargeEdge, bz - 1].Id == airId) ||
                                (bz < sectionLargeEdge && nowSection.Blocks[bx, sectionLargeEdge, bz + 1].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[bx, sectionLargeEdge, bz];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                    //------------------------------------------------//
                    // Z small edge in the section
                    for (int bx = 0; bx <= sectionLargeEdge; bx++)
                    {
                        for (int by = 0; by <= sectionLargeEdge; by++)
                        {
                            if (sz == 0 || // on the edge of whole map
                                nowSection.Blocks[bx, by, sectionSmallEdge + 1].Id == airId ||  // Znext is air

                                (yLastSection != null && zLastSection.Blocks[bx, by, sectionLargeEdge].Id == airId) || // block in the Z Last Section is air

                                (bx > 0 && nowSection.Blocks[bx - 1, by, sectionSmallEdge].Id == airId) ||
                                (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, by, sectionSmallEdge].Id == airId) ||
                                (by > 0 && nowSection.Blocks[bx, by - 1, sectionSmallEdge].Id == airId) ||
                                (by < sectionLargeEdge && nowSection.Blocks[bx, by + 1, sectionSmallEdge].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[bx, by, sectionSmallEdge];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                    // Z large edge in the section
                    for (int bx = 0; bx <= sectionLargeEdge; bx++)
                    {
                        for (int by = 0; by <= sectionLargeEdge; by++)
                        {
                            if (sz == sectionsLenZ - 1 || // On the edge of whole map
                                nowSection.Blocks[bx, by, sectionLargeEdge - 1].Id == airId ||  // Zlast is air

                                (xNextSection != null && zNextSection.Blocks[bx, by, sectionSmallEdge].Id == airId) || // block in the Z Next Section is air

                                (bx > 0 && nowSection.Blocks[bx - 1, by, sectionLargeEdge].Id == airId) ||
                                (bx < sectionLargeEdge && nowSection.Blocks[bx + 1, by, sectionLargeEdge].Id == airId) ||
                                (by > 0 && nowSection.Blocks[bx, by - 1, sectionLargeEdge].Id == airId) ||
                                (by < sectionLargeEdge && nowSection.Blocks[bx, by + 1, sectionLargeEdge].Id == airId))
                            {
                                // If the block is visible, create it at once
                                Block nowBlock = nowSection.Blocks[bx, by, sectionLargeEdge];

                                this._blockCreator.CreateBlock(nowBlock);
                            }
                        }
                    }
                }
            }
        }
    }
}