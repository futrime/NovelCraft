using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public const int SlotNum = 36;
    public Slot[] Slots;
    public Inventory()
    {
        Slots = new Slot[SlotNum];
        for (int i = 0; i < Slots.Count(); i++)
        {
            Slots[i] = new Slot();
            Slots[i].SlotIndex = i;
        }
    }
    public void SwapSlot(int slotA, int slotB)
    {
        Slot temp = Slots[slotA];
        Slots[slotA] = Slots[slotB];
        Slots[slotB] = temp;
    }
}
