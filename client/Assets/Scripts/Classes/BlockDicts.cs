using System;
using System.Collections.Generic;
using System.Linq;
public class BlockDicts
{
    /// <summary>
    /// The block dict <string name, int id>
    /// </summary>
    public static Dictionary<string, int> BlockDict = JsonUtility.ParseBlockDictJson("Json/Dict");
    /// <summary>
    /// Get the block name using block id
    /// </summary>
    public static string[] BlockNameArray = DictUtility.BlockDictParser(BlockDict);

}

