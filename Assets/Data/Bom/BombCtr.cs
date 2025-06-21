using UnityEngine;

public class BombCtr : GemCtr
{
    [SerializeField] private GemBoardCtr gemBoardCtr;
    [SerializeField] protected BomMove bomMove;
    public BomMove BomMove => bomMove;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadBomMove();
        this.LoadGemBoardCtr();
        this.GemType = GemType.Bomb;
    }
    protected virtual void LoadBomMove()
    {
        if (this.bomMove != null) return;
        this.bomMove = GetComponentInChildren<BomMove>();
        Debug.Log(transform.name + " :LoadBomMove", gameObject);
    }
    protected virtual void LoadGemBoardCtr()
    {
        if (this.gemBoardCtr != null) return;
        this.gemBoardCtr = FindObjectOfType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemBoardCtr", gameObject);
    }
    public void Explode()
    {
        Debug.Log("💣 Bomb exploded at: " + xIndex + "," + yIndex);

        Node[,] board = gemBoardCtr.Gemboard.gemBoardNode;

        int width = board.GetLength(0);
        int height = board.GetLength(1);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = xIndex + dx;
                int y = yIndex + dy;

                if (x < 0 || y < 0 || x >= width || y >= height) continue;

                Node node = board[x, y];
                if (node.Gem != null)
                {
                    GemCtr gem = node.Gem.GetComponent<GemCtr>();
                    if (gem != null && !gem.ItMatched)
                    {
                        gem.ItMatched = true;
                        GemSpawnCtr.Instance.GemSpawner.GemtoRemove.Add(gem);
                    }
                }
            }
        }
       
        // Đừng quên thêm chính bomb vào danh sách xoá
        if (!this.ItMatched)
        {
            this.ItMatched = true;
            GemSpawnCtr.Instance.GemSpawner.GemtoRemove.Add(this);
        }
    }
    
}

