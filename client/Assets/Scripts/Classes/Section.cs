using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Level;

public class Section
{
    /// <summary>
    /// shape: (16,16,16)
    /// </summary>
    public Block[,,] Blocks;
    /// <summary>
    /// The absolute position in the map / 16
    /// </summary>
    public Vector3Int PositionIndex;
    public Section(Vector3Int positionIndex)
    {
        this.Blocks = new Block[LevelInfo.SectionLength, LevelInfo.SectionLength, LevelInfo.SectionLength];
        this.PositionIndex = positionIndex;
    }
}
