using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FxDespawn : DespawnByTime
{
    
    public override void DespawnObject()
    {
        FxSpawer.Instance.Despawn(transform.parent);
    }
    protected override void ResetValue()
    {
        base.ResetValue();
        this.delay = 0.2f;
    }

}

