using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GemDespawn : NghiaMono
{
    
    public virtual void DespawnObject()
    {
        GemSpawner.Instance.Despawn(transform.parent);
       //Destroy(transform.parent.gameObject);
       this.OnDeadFx();
    }
    protected virtual void OnDeadFx()
    {
        string fxname = this.GetOnDeadFxname();
        Transform fxOnDead = FxSpawer.Instance.Spawn(fxname, transform.position, transform.rotation);
        fxOnDead.gameObject.SetActive(true);
    }
    protected virtual string GetOnDeadFxname()
    {
        return FxSpawer.Smokeone;
    }
}

