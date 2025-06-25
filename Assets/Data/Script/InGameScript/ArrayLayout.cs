using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout 
{

    [System.Serializable]
    public class Row
    {
        public bool[] row;
    }

    public Row[] rows = new Row[0];
}
