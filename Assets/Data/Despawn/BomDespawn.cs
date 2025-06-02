using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BomDespawn : Despawn
{
    
    public override void DespawnObject()
    {
       // GemSpawner.Instance.Despawn(transform.parent);
       Destroy(transform.parent.gameObject);
       
    }
}

