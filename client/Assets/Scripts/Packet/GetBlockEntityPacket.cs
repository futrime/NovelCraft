using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class GetBlockEntityPacket : Packet
{
    private List<Section> _sections;
    public List<Section> Sections
    {
        get
        {
            return _sections;
        }
    }
    public List<Entity> _entities;
    public List<Entity> Entities
    {
        get
        {
            return _entities;
        }
    }
    public override JObject GetPacket()
    {
        JObject jsonPacket = new JObject();

        jsonPacket["bound_to"] = "serverbound";
        jsonPacket["type"] = "get_blocks_and_entities_request";

        return jsonPacket;
    }
    public override bool ParsePacket(JObject serverPacket)
    {
        // Check type
        JToken typeToken = serverPacket["type"];
        if (typeToken == null || typeToken.ToString() != "get_blocks_and_entities_response")
            return false;

        // Sections
        JToken sectionsToken = serverPacket["sections"];
        if (sectionsToken == null)
            return false;

        this._sections = new();
        foreach (JToken sectionToken in sectionsToken)
        {
            int sx = int.Parse(sectionToken["x"].ToString());
            int sy = int.Parse(sectionToken["y"].ToString());
            int sz = int.Parse(sectionToken["z"].ToString());

            this._sections.Add(new Section(new Vector3Int(sx, sy, sz)));

            JArray blocksToken = (JArray)sectionToken["blocks"];
            for (int j = 0; j < blocksToken.Count; j++)
            {
                // Compute relative position <The blocks in the section which can be accessed by `blocks[x*256+y*16+z]>
                int bx = j / 256;
                int by = j / 16 - bx * 16;
                int bz = j % 16;

                short blockId = short.Parse(blocksToken[j].ToString());

                Block nowBlock = this._sections.Last().Blocks[bx, by, bz];

                nowBlock.Id = blockId;
                nowBlock.Position = new Vector3Int(sx + bx, sy + by, sz + bz);
                nowBlock.Name = BlockDicts.BlockNameArray[blockId];
            }
        }

        // Entities
        JToken entitiesToken = serverPacket["entities"];
        if (entitiesToken == null)
            return false;
        foreach (JToken entityToken in entitiesToken)
        {
            Entity entity = new()
            {
                UniqueId = int.Parse(entityToken["unique_id"].ToString())
            };

            JToken PositionToken = entityToken["position"];
            int x = int.Parse(PositionToken["x"].ToString());
            int y = int.Parse(PositionToken["y"].ToString());
            int z = int.Parse(PositionToken["z"].ToString());
            entity.Position = new Vector3Int(x, y, z);

            JToken orientationToken = entitiesToken["orientation"];
            entity.yaw = int.Parse(orientationToken["yaw"].ToString());
            entity.pitch = int.Parse(orientationToken["pitch"].ToString());

            this._entities.Add(entity);
        }

        return true;
    }
}
