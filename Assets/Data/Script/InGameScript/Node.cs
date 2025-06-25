using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node
{
    public bool isUsable;
    public GameObject Gem;
    public TileCtr Tile;
    public Node(bool isUsable, GameObject gem, TileCtr tile = null)
    {
        this.isUsable = isUsable;
        this.Gem = gem;
        this.Tile = tile;
    }
    public bool IsBlocked()
    {
        return Tile != null && Tile.IsBlockingGem();
    }
    public bool CanAcceptGem()
    {
        return Gem == null && (Tile == null || !Tile.IsBlockingGem());
    }
}