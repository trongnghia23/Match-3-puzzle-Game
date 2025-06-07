using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout 
{
    [System.Serializable]

    public struct rowdata
    {
        public bool[] row;

    }
    public rowdata[] rows = new rowdata[14];
}
