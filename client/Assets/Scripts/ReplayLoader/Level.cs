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
public class Level : MonoBehaviour
{
    public class LevelInfo
    {
        public const int SectionLength = 16;
        public const int BlockNumInSections = SectionLength * SectionLength * SectionLength;
    }
    private BlockCreator _blockCreator;
    private LevelInfo _levelInfo;
    private Upload.OpenFileName _levelFile = new() { };

    /// <summary>
    /// Get the private _levelInfo
    /// </summary>
    public LevelInfo LevelInformation
    {
        get { return _levelInfo; }
    }
    private void Start()
    {
        // Initialize the _recordInfo
        this._levelInfo = new();
        // Initialize the BlockCreator
        this._blockCreator = GameObject.Find("BlockCreator").GetComponent<BlockCreator>();
        // Get json file
        var fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();
        // Check if the file is Level json
        this._levelFile = fileLoaded.File;

        Run();
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
        CheckBlocksVisibility();
    }
    public void LoadBlockData()
    {
        // Read the json file and Process the replay
        JObject jsonObject = (JsonUtility.UnzipLevel(this._levelFile.File));
        // Deal with Sections: array
        JArray sections = (JArray)jsonObject["sections"];

        for (int i = 0; i < sections.Count; i++)
        {
            // Get the absolute position of now section
            int sectionX = int.Parse(sections[i]["x"].ToString());
            int sectionY = int.Parse(sections[i]["y"].ToString());
            int sectionZ = int.Parse(sections[i]["z"].ToString());

            // All blocks in one section
            Section section = new(new Vector3Int(sectionX, sectionY, sectionZ) / LevelInfo.SectionLength);

            // jsonSection: array<int blockID>
            JArray jsonSection = (JArray)(sections[i]["blocks"]);
            if (jsonSection.Count != LevelInfo.BlockNumInSections)
            {
                throw new System.Exception($"The length per section is not {LevelInfo.BlockNumInSections}");
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
                    section.Blocks[x, y, z].Name = BlockDicts.BlockNameArray[section.Blocks[x, y, z].Id];
                }
                catch
                {
                    //Debug.Log(BlockDicts.BlockNameArray);
                    //Debug.Log(section.Blocks[x, y, z].Id);
                    section.Blocks[x, y, z].Name = "";
                    section.Blocks[x, y, z].Id = -1;
                }
                // Compute absolute position
                section.Blocks[x, y, z].Position = new Vector3Int(sectionX + x, sectionY + y, sectionZ + z);
            }
            //try
            //{
            BlockSource.AddSection(section);
            //}
            //catch
            //{
            //    Debug.Log(i);
            //    Debug.Log(new Vector3Int(sectionX, sectionY, sectionZ));
            //}
        }
        //Debug.Log($"Section num: {this._levelInfo.AllBlockSource.SectionDict.Count}");
    }
    public void CheckBlocksVisibility()
    {
        CheckVisibility.CheckInnerVisibility(this._blockCreator);
        CheckVisibility.CheckNeighbourVisibility(this._blockCreator);
    }
}