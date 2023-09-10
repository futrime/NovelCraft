using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour
{
    public static Dictionary<string, int> BlockPrefabDict = new Dictionary<string, int>{
        {"Air",0},
        {"Stone",1 },
        {"Grass",2 },
        {"Dirt",3 },
        {"Planks",4 },
        {"Bedrock",5 },
        {"Log",6 },
        {"Leaves",7 },
        {"Cobblestone",8 },
        {"CoalOre",9 },
        {"IronOre",10},
        {"GoldenOre",11 },
        {"DiamondOre",12 },
    };
    public GameObject[] BlockPrefabs;  // Find in the all prefabs

    public string MissingTextureBlockName = "MissingTextureBlock";
    public GameObject MissingTextureBlock; // Use this when the texture is not found

    // Start is called before the first frame update
    void Start()
    {
        // Load all the Block prefabs
        BlockPrefabs = new GameObject[BlockPrefabDict.Count];

        foreach (var prefabInfo in BlockPrefabDict)
        {
            string name = prefabInfo.Key;
            int index = prefabInfo.Value;
            BlockPrefabs[index] = Resources.Load<GameObject>($"Blocks/{name}/{name}");
        }
        // Load Missing texture block
        MissingTextureBlock = Resources.Load<GameObject>($"Blocks/{MissingTextureBlockName}/{MissingTextureBlockName}");
    }
    /// <summary>
    /// Create a block in the unity (make the block become a GameObject)
    /// </summary>
    /// <param name="block">The block to be created in the Unity</param>
    /// <returns></returns>
    public bool CreateBlock(Block block)
    {
        // Create the block if the block hasn't been created
        if (block.BlockObject != null)
            return false;
        // Create the block if the block is not air
        if (block.Id == 0)
            return false;

        // The block object to be created
        GameObject blockObject;

        if (BlockPrefabDict.ContainsKey(block.Name))
        {
            // Get the index in array "BlockPrefabs"
            int prefabIndex = BlockPrefabDict[block.Name];
            // Create block object
            blockObject = (GameObject)Instantiate(BlockPrefabs[prefabIndex]);
        }
        else
        {
            // Miss texture
            blockObject = (GameObject)Instantiate(MissingTextureBlock);
        }

        block.BlockObject = blockObject;
        // Put the object in a right position, its parent is 'BlockCreator' object
        block.BlockObject.transform.parent = this.transform;
        // The center of block is (0.5+x, 0.5+y, 0.5+z)
        block.BlockObject.transform.position = new Vector3(block.Position.x + 0.5f, block.Position.y + 0.5f, block.Position.z + 0.5f);

        // Create the cube successfully!
        return true;
    }
    /// <summary>
    /// Destroy block object if it's not null
    /// </summary>
    /// <param name="block"></param>
    /// <returns>True if the object is deleted successfully</returns>
    public bool DestroyBlock(Block block)
    {
        if (block.BlockObject == null)
            return false;

        Destroy(block.BlockObject);
        return true;
    }
    /// <summary>
    /// Update the block according to the position
    /// </summary>
    /// <param name="position">The absolute position of the block to be updated</param>
    /// <param name="blockId">New block id</param>
    /// <param name="blockName">New block name</param>
    /// <returns>null if the block cannot be updated</returns>
    public Block UpdateBlock(Vector3Int position, short blockId, string blockName, out short? originalBlockId)
    {
        Block block = BlockSource.GetBlock(position);
        // False if the block cannot be found or its gameobject hasn't already created
        if (block == null)
        {
            originalBlockId = null;
            return null;
        }
        originalBlockId = block.Id;

        // Reconstruct the block
        if (blockId != originalBlockId)
        {
            DestroyBlock(block);

            // Update info
            block.Id = blockId;
            block.Name = blockName;

            CreateBlock(block);
        }
        return block;
    }
}
