using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout 
{
    public Levels currentLevel;

    [System.Serializable]
    public class Row
    {
        public bool[] row;
    }

    public Row[] rows = new Row[14];
}
