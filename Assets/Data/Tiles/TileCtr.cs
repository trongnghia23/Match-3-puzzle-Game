using System.Collections.Generic;
using UnityEngine;

public class TileCtr : NghiaMono
{
    [Header("Tile Info")]
    public TileType tileType = TileType.None;
    public int TilesHp;
    public int MaxHp { get; private set; }
    public TileDespawn TileDespawn;
   
    public Node node;
    public int xIndex, yIndex;
    private int moveCounter = 0;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        LoadTileDespawn();
    }

    
    protected virtual void LoadTileDespawn()
    {
        if (TileDespawn != null) return;
        TileDespawn = GetComponentInChildren<TileDespawn>();
    }
    public void Init(TileType type, int hp = -1)
    {
        tileType = type;

        // Nếu hp < 0 ⇒ dùng HP mặc định theo tileType
        TilesHp = hp > 0 ? hp : GetDefaultHp(type);
        MaxHp = TilesHp;
    }
    public void SetNode(Node n, int x, int y)
    {
        node = n;
        xIndex = x;
        yIndex = y;
    }
    public bool IsBlockingGem() => TilesHp > 0;


    public virtual void Hit()
    {
        if (TilesHp > 0) TilesHp--;
    }
    private int GetDefaultHp(TileType type) => type switch
    {
        TileType.IceTile => 1,
        TileType.LockTile => 1,
        TileType.StoneTile => 5,
        TileType.SlimeTile => 1,
        _ => 0
    };
    public virtual bool OnPlayerMove()          // ← bool
    {
        if (tileType != TileType.SlimeTile || TilesHp <= 0) return false;

        moveCounter++;
        if (moveCounter % 2 != 0) return false; // chỉ mỗi 2 lượt

        return SpreadOneTile();                 // trả về true nếu đã lan
    }
    private static readonly Vector2Int[] dirs =
       { new(0,1), new(0,-1), new(1,0), new(-1,0) };

    private bool SpreadOneTile()
    {
        List<Vector2Int> empty = new();

        foreach (var d in dirs)
        {
            int nx = xIndex + d.x;
            int ny = yIndex + d.y;

            if (!Gemboard.Instance.InBounds(nx, ny)) continue;

            Node n = Gemboard.Instance.gemBoardNode[nx, ny];

            // CHỈ lan ra ô có GEM và chưa có TILE
            if (n.Tile == null && n.Gem != null)
            {
                empty.Add(new Vector2Int(nx, ny));
            }
        }

        if (empty.Count == 0) return false;

        Vector2Int target = empty[Random.Range(0, empty.Count)];
        TileSpawner.Instance.SpawnTile(TileType.SlimeTile, target);
        moveCounter = 0;
        return true;
    }


}




public enum TileType
{
    None,
    IceTile,
    LockTile,
    StoneTile,
    SlimeTile
    // có thể thêm: Stone, Fire, Poison, ...
}

