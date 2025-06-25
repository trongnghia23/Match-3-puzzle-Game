using UnityEngine;

[System.Serializable]
public class TileMapLayout
{
    [System.Serializable]
    public class Row
    {
        public TileType[] row;
    }

    public Row[] rows = new Row[0];
}