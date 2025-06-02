using UnityEngine;

public class BomCtr : GemCtr
{
    [SerializeField] protected BomSpawnerCtr bomSpawnerCtr;
    [SerializeField] protected BomMove bomMove;
    [SerializeField] protected BomDespawn bomDespawn;

    public BomCtr(int x, int y) : base(x, y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadBomSpawnerCtr();
        this.LoadBomMove();
        this.LoadBomDespawn();
    }

    protected virtual void LoadBomSpawnerCtr()
    {
        if (this.bomSpawnerCtr != null) return;
        this.bomSpawnerCtr = Transform.FindAnyObjectByType<BomSpawnerCtr>();
    }

    protected virtual void LoadBomMove()
    {
        if (this.bomMove != null) return;
        this.bomMove = GetComponentInChildren<BomMove>();
    }

    protected virtual void LoadBomDespawn()
    {
        if (this.bomDespawn != null) return;
        this.bomDespawn = GetComponentInChildren<BomDespawn>();
    }

    public int explosionRadius = 1; // How many gems in each direction to destroy

    public void Explode(GemCtr gemCtr)
    {
        // Get all gems in explosion radius
        for (int x = gemCtr.xIndex - explosionRadius; x <= gemCtr.xIndex + explosionRadius; x++)
        {
            for (int y = gemCtr.yIndex - explosionRadius; y <= gemCtr.yIndex + explosionRadius; y++)
            {
                if (x >= 0 && x < bomSpawnerCtr.GemboardCtr.Gemboard.width && y >= 0 && y < bomSpawnerCtr.GemboardCtr.Gemboard.height)
                {
                    if (bomSpawnerCtr.GemboardCtr.Gemboard.gemBoardNode[x, y].Gem != null)
                    {
                        GemCtr gem = bomSpawnerCtr.GemboardCtr.Gemboard.gemBoardNode[x, y].Gem.GetComponent<GemCtr>();
                        if (gem != null)
                        {
                            gem.ItMatched = true;
                            bomSpawnerCtr.GemSpawner.GemtoRemove.Add(gem);
                        }
                    }
                }
            }
        }
    }
} 