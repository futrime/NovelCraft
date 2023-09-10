using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpdateEntities : Packet
{
    private List<Entity> _entities = new();
    public List<Entity> Entities
    {
        get { return _entities; }
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
        if (typeToken == null || typeToken.ToString() != "update_entity_changes")
            return false;

        // Entities
        JToken entitiesToken = serverPacket["entities"];
        if (entitiesToken == null)
            return false;

        foreach (JToken entityToken in entitiesToken)
        {
            Entity entity = new()
            {
                UniqueId = int.Parse(entityToken["unique_id"].ToString()),
                EntityId = int.Parse(entityToken["entity_id"].ToString()),
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

