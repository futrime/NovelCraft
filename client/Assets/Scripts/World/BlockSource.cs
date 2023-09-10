using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

public class BlockSource
{
    /// <summary>
    /// Key : <Vector3Int section.positionIndex> which is equal to section's position divided by 16
    /// </summary>
    public static Dictionary<Vector3Int, Section> SectionDict = new Dictionary<Vector3Int, Section>();

    /// <summary>
    /// Add section into the dict
    /// </summary>
    /// <param name="section"></param>
    /// <returns>False if the section in the position already exists</returns>
    public static bool AddSection(Section section)
    {
        if (SectionDict.ContainsKey(section.PositionIndex))
        {
            return false;
        }
        else
        {
            SectionDict.Add(section.PositionIndex, section);
            return true;
        }
    }
    /// <summary>
    /// Get block by using absolute position
    /// </summary>
    /// <param name="position">Block absolute position</param>
    /// <returns></returns>
    public static Block GetBlock(Vector3Int position)
    {
        Vector3Int sectionPositionIndex = GetSectionPositionIndex(position);
        if (SectionDict.ContainsKey(sectionPositionIndex))
        {
            // The relative position to now section
            Vector3Int relativePosition = position - sectionPositionIndex * 16;
            return SectionDict[sectionPositionIndex].Blocks[relativePosition.x, relativePosition.y, relativePosition.z];
        }
        else
        {
            // Cannot find the block
            return null;
        }
    }

    /// <summary>
    /// Get index of the section
    /// </summary>
    /// <param name="blockPosition"></param>
    /// <returns></returns>
    private static Vector3Int GetSectionPositionIndex(Vector3 blockPosition)
    {
        return Vector3Int.FloorToInt(new Vector3(blockPosition.x, blockPosition.y, blockPosition.z) / 16.0f);
    }
}
