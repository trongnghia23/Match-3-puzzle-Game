using UnityEngine;

public class BomSpawner : spawner
{
   protected static BomSpawner instance;
    public static BomSpawner Instance { get => instance; }
    [SerializeField] protected BomSpawnerCtr bomSpawnerCtr;
    [SerializeField] protected GemBoardCtr gemboardCtr;
   
    protected override void Awake()
    {
        base.Awake();
        if (BomSpawner.instance != null) Debug.LogError("only one BomSpawner allow to exist");
        BomSpawner.instance = this;
    }

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadBomSpawnerCtr();
        this.LoadGemboardCtr();
    }

    protected virtual void LoadBomSpawnerCtr()
    {
        if (this.bomSpawnerCtr != null) return;
        this.bomSpawnerCtr = GetComponent<BomSpawnerCtr>();
        Debug.Log(transform.name + " :LoadBomSpawnerCtr", gameObject);
    }

    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
    }
}
