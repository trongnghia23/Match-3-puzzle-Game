using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Despawn : NghiaMono
{
    protected virtual void FixedUpdate()
    {
        this.Despawning();
    }

    protected virtual void Despawning()
    {
        if (!this.CanDespawn()) return;
        this.DespawnObject();
    }
    public virtual void DespawnObject()
    {
        Destroy(transform.parent.gameObject);
    }

    public virtual bool CanDespawn()
    {
        return false;
    }

}
