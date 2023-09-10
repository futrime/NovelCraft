using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EntitySource
{
    /// <summary>
    /// The allItemDict store all the items in the game 
    /// </summary>
    public static Dictionary<int, Item> ItemDict = new();
    public static Dictionary<int, Player> PlayerDict = new();
    public static bool AddItem(Item item)
    {
        if (ItemDict.ContainsKey(item.UniqueId))
            return false;

        if (item.EntityObject == null)
            return false;

        ItemDict.Add(item.UniqueId, item);
        return true;
    }
    public static Item GetItem(int uniqueId)
    {
        if (ItemDict.ContainsKey(uniqueId))
        {
            return ItemDict[uniqueId];
        }
        else
        {
            return null;
        }
    }
    public static bool AddPlayer(Player player)
    {
        if (PlayerDict.ContainsKey(player.UniqueId))
            return false;

        if (player.EntityObject == null)
            return false;

        PlayerDict.Add(player.UniqueId, player);
        return true;
    }
    public static Player GetPlayer(int uniqueId)
    {
        if (PlayerDict.ContainsKey(uniqueId))
        {
            return PlayerDict[uniqueId];
        }
        else
        {
            return null;
        }
    }

    public static Entity GetEntity(int uniqueId, out int? entityTypeId)
    {
        entityTypeId = null;
        Entity entity = EntitySource.GetItem(uniqueId);
        if (entity != null)
        {
            entityTypeId = 1;
            return entity;
        }

        entity = EntitySource.GetPlayer(uniqueId);
        if (entity != null)
            entityTypeId = 0;

        return entity;
    }
}
