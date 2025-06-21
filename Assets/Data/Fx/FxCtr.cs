using UnityEngine;

public class FxCtr : NghiaMono
{
    [SerializeField] protected FxDespawn fxDespawn;
    public FxDespawn FxDespawn => fxDespawn;

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadFxDespawn();
    }

    protected virtual void LoadFxDespawn()
    {
        if (this.fxDespawn != null) return;
        this.fxDespawn = GetComponentInChildren<FxDespawn>();
        Debug.Log(transform.name + " :LoadFxDespawn", gameObject);
    }
    
}
