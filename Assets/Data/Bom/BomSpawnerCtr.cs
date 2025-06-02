using UnityEngine;

public class BomSpawnerCtr : NghiaMono
{
    [SerializeField] protected BomSpawner bomSpawner;
    public BomSpawner BomSpawner { get => bomSpawner; }
    [SerializeField] protected Transform bomHolder;
    public Transform BomHolder => bomHolder;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    public GemBoardCtr GemboardCtr => gemboardCtr;
   
   [SerializeField] protected GemSpawner gemSpawner;
   public GemSpawner GemSpawner => gemSpawner;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadBomSpawner();
        this.LoadHodler();
        this.LoadGemboardCtr();
        this.LoadGemSpawner();
    }

    protected virtual void LoadBomSpawner()
    {
        if (this.bomSpawner != null) return;
        this.bomSpawner = GetComponent<BomSpawner>();
        Debug.Log(transform.name + " :LoadBomSpawner", gameObject);
    }

    protected virtual void LoadHodler()
    {
        if (this.bomHolder != null) return;
        this.bomHolder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHodler", gameObject);
    }
    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
    }
    protected virtual void LoadGemSpawner()
    {
        if (this.gemSpawner != null) return;
        this.gemSpawner = Transform.FindAnyObjectByType<GemSpawner>();
    }
    
}
