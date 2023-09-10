using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class GetPlayerInformationPacket : Packet
{
    private Player _player;
    public Player PlayerInfo { get { return _player; } }
    public override JObject GetPacket()
    {
        JObject jsonPacket = new JObject();

        jsonPacket["bound_to"] = "serverbound";
        jsonPacket["type"] = "get_player_information_request";

        return jsonPacket;
    }
    public override bool ParsePacket(JObject serverPacket)
    {
        // Check type
        JToken typeToken = serverPacket["type"];
        if (typeToken == null || typeToken.ToString() != "get_player_information_response")
            return false;

        this._player = new();
        // Health
        JToken healthToken = serverPacket["health"];
        if (healthToken == null) return false;

        this._player.Health = int.Parse(healthToken.ToString());

        // Experiments
        JToken experimentsToken = serverPacket["experiments"];
        if (experimentsToken == null) return false;

        this._player.Experiments = int.Parse(experimentsToken.ToString());

        // Inventory
        JToken inventoryToken = serverPacket["inventory"];

        foreach (JToken slotToken in inventoryToken)
        {
            int slot = int.Parse(slotToken["slot"].ToString());
            int itemId = int.Parse(slotToken["item_id"].ToString());
            int count = int.Parse(slotToken["count"].ToString());
            int damage = 0;
            JToken damageToken = slotToken["damage"];
            if (damageToken != null)
            {
                damage = int.Parse(damageToken.ToString());
            }

            this._player.Inventory.Slots[slot] = new(slot, itemId, count, damage);
        }

        // Main hand
        this._player.MainHandSlot = int.Parse(serverPacket["main_hand"].ToString());
        return true;
    }
}

