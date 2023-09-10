using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UpdateBlocksPacket : Packet
{
    private List<Block> _blocks = new();
    public List<Block> Blocks
    {
        get { return _blocks; }
    }
    public override JObject GetPacket()
    {
        // There is no need to send packet to server
        return null;
    }
    public override bool ParsePacket(JObject serverPacket)
    {
        // Check type
        JToken typeToken = serverPacket["type"];
        if (typeToken == null || typeToken.ToString() != "update_block_changes")
            return false;

        this._blocks.Clear();
        JToken blocksToken = serverPacket["blocks"];
        if (blocksToken == null)
            return false;

        foreach (JToken blockToken in blocksToken)
        {
            // Absolute position
            int x = int.Parse(blockToken["x"].ToString());
            int y = int.Parse(blockToken["y"].ToString());
            int z = int.Parse(blockToken["z"].ToString());
            short blockId = short.Parse(blockToken["block_id"].ToString());

            Block block = new(id: blockId, new Vector3Int(x, y, z));

            this._blocks.Add(block);
        }

        return true;
    }
}

