using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot
{
    public const int NullItemId = 0;
    public int SlotIndex;
    public int ItemId;
    public int Count
    {
        get { return _count; }
        set
        {
            if (value == 0)
            {
                this.ItemId = NullItemId;
                _count = value;
            }
        }
    }
    public int Damage;

    private int _count;
    public Slot(int slotIndex, int itemId, int count, int damage)
    {
        this.SlotIndex = slotIndex;
        this.ItemId = itemId;
        this.Count = count;
        this.Damage = damage;
    }
    public Slot()
    {
        this.SlotIndex = 0;
        this.ItemId = NullItemId;
        this.Count = 0;
        this.Damage = 0;
    }
}


