using UnityEngine;

public class GemSpawnCtr : NghiaMono
{

    [SerializeField] protected GemSpawner gemSpawner;
    public GemSpawner GemSpawner { get => gemSpawner; }
    [SerializeField] protected Transform holder;
    public Transform Holder => holder;
   
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemSpawner();
        this.LoadHodler();
    }

    protected virtual void LoadGemSpawner()
    {
        if (this.gemSpawner != null) return;
        this.gemSpawner = GetComponent<GemSpawner>();
        Debug.Log(transform.name + " :LoadGemSpawner", gameObject);
    }

    protected virtual void LoadHodler()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHodler", gameObject);
    }
    
    
}
