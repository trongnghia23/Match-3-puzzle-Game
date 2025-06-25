using UnityEngine;

public class TileSpawner : spawner
{
    protected static TileSpawner instance;
    public static TileSpawner Instance { get => instance; }
    [Header("Tile Layout - from Levels.asset")]
    [SerializeField] protected TileMapLayout tileMapLayout;

    [Header("Tile Parent")]
    [SerializeField] protected GemBoardCtr gemboardCtr;
    protected override void Awake()
    {
        base.Awake();
        if (TileSpawner.instance != null) Debug.LogError("only one TileSpawner allow to exist");
        TileSpawner.instance = this;
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboardCtr();
    }

    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemboardCtr", gameObject);
    }

    public void SetTileMapLayout(TileMapLayout layout)
    {
        this.tileMapLayout = layout;
    }

    public virtual void SpawnAllTiles()
    {
        if (tileMapLayout == null || tileMapLayout.rows.Length == 0)
        {
            Debug.LogWarning("TileSpawner: TileMapLayout rỗng hoặc null.");
            return;
        }

        int height = tileMapLayout.rows.Length;
        int width = tileMapLayout.rows[0].row.Length;



        for (int y = 0; y < height; y++) // y duyệt dòng
        {
            for (int x = 0; x < width; x++) // x duyệt cột
            {
                if (x >= gemboardCtr.Gemboard.width || y >= gemboardCtr.Gemboard.height)
                    continue;

                TileType type = tileMapLayout.rows[y].row[x]; // đảo lại x <-> y
                if (type == TileType.None) continue;

                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 spawnPos = Gemboard.Instance.GetWorldPos(gridPos);
                Node node = gemboardCtr.Gemboard.gemBoardNode[x, y];

                Transform tileTransform = this.SpawnByEnum(type, spawnPos, Quaternion.identity, this.holder);
                if (tileTransform == null) continue;

                tileTransform.gameObject.SetActive(true);

                TileCtr tileCtr = tileTransform.GetComponent<TileCtr>();
                if (tileCtr != null)
                {
                    // Gọi Init để tile tự biết mình là loại nào và HP bao nhiêu
                    tileCtr.Init(type); // tự lấy HP mặc định (1, 2, 3,...)
                    tileCtr.SetNode(node, x, y );
                    node.Tile = tileCtr;
                }
            }
        }

    }

    public Transform SpawnTile(TileType type, Vector2Int gridPos)
    {
        if (type == TileType.None) return null;

        if (gridPos.x < 0 || gridPos.y < 0 ||
            gridPos.x >= gemboardCtr.Gemboard.width ||
            gridPos.y >= gemboardCtr.Gemboard.height)
            return null;

        Vector3 spawnPos = Gemboard.Instance.GetWorldPos(gridPos);
        Node node = gemboardCtr.Gemboard.gemBoardNode[gridPos.x, gridPos.y];

        if (node.Tile != null) return null; // Chặn nếu đã có Tile (Ice/Lock...)


        Transform tileTransform = this.SpawnByEnum(type, spawnPos, Quaternion.identity, this.holder);
        if (tileTransform == null) return null;

        tileTransform.gameObject.SetActive(true);
        TileCtr tileCtr = tileTransform.GetComponent<TileCtr>();
        if (tileCtr != null)
        {
            tileCtr.Init(type);
            tileCtr.SetNode(node, gridPos.x, gridPos.y);
            node.Tile = tileCtr;
        }

        return tileTransform;
    }

    public virtual void ClearAllTiles()
    {
        foreach (Transform child in this.holder)
        {
            Destroy(child.gameObject);
        }
    }
}
