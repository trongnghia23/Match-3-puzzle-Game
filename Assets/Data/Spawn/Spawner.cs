using System.Collections.Generic;
using UnityEngine;

public class spawner : NghiaMono
{
    [SerializeField] protected Transform holder;
    [SerializeField] protected List<Transform> prefabs;
    [SerializeField] protected List<Transform> poolObjs;
    [SerializeField]protected int spawnCount = 0;
    public int Spawncount => spawnCount;
    
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadHolder();
        this.LoadPrefabs();
    }

    protected virtual void LoadHolder()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHolder", gameObject);
    }

    protected virtual void LoadPrefabs()
    {
        if (this.prefabs.Count > 0) return;
        Transform prefabObj = transform.Find("Prefabs");
        foreach (Transform prefab in prefabObj)
        {
            this.prefabs.Add(prefab);
        }
        this.HidePrefabs();
        Debug.Log(transform.name + ": LoadPrefabs", gameObject);
    }

    protected virtual void HidePrefabs()
    {
        foreach (Transform prefab in this.prefabs)
        {
            prefab.gameObject.SetActive(false);
        }
    }

    public virtual Transform RandomPrefab()
    {
        int rand = Random.Range(0, this.prefabs.Count);
        return this.prefabs[rand];

    }
    public virtual void Spawncountup()
    {
        this.spawnCount++;
    }

    public virtual void Spawncountdown()
    {
        this.spawnCount--;
    }

   public virtual Transform Spawn(Transform prefab, Vector3 spawnPos, Quaternion rotation, Transform parent)
{
    Transform newPrefab = this.GetObjectFromPool(prefab);
    newPrefab.SetPositionAndRotation(spawnPos, rotation);
    newPrefab.parent = parent != null ? parent : this.holder;
    newPrefab.gameObject.SetActive(true);
    this.spawnCount++;
    return newPrefab;
}
    public virtual Transform Spawn(string prefabName, Vector3 spawnPos, Quaternion rotation, Transform parent = null)
    {
        Transform prefab = GetPrefabByName(prefabName);
        if (prefab == null)
        {
            Debug.LogWarning("Prefab not found: " + prefabName);
            return null;
        }

        return this.Spawn(prefab, spawnPos, rotation, parent);
    }
    public virtual Transform SpawnByEnum(System.Enum enumVal, Vector3 spawnPos, Quaternion rotation, Transform parent = null)
    {
        return Spawn(enumVal.ToString(), spawnPos, rotation, parent);
    }
    public virtual void Despawn(Transform obj)
    {
        this.poolObjs.Add(obj);
        obj.gameObject.SetActive(false);
        this.spawnCount--;
    }

    protected virtual Transform GetObjectFromPool(Transform prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("GetObjectFromPool: prefab is null or destroyed.");
            return null;
        }
        for (int i = 0; i < poolObjs.Count; i++)
        {
            Transform poolObj = poolObjs[i];
            if (poolObj.name == prefab.name && !poolObj.gameObject.activeInHierarchy)
            {
                poolObjs.RemoveAt(i);
                return poolObj;
            }
        }
        if (prefab == null)
    {
            Debug.LogError("Prefab was destroyed before Instantiate could be called.");
            return null;
        }
        Transform newPrefab = Instantiate(prefab);
        newPrefab.name = prefab.name;
        return newPrefab;
    }
    public virtual Transform GetPrefabByEnum(System.Enum enumValue)
    {
        string name = enumValue.ToString();
        return GetPrefabByName(name);
    }
    public virtual Transform GetPrefabByName(string prefabName)
    {
        foreach (Transform prefab in this.prefabs)
        {
            if (prefab.name == prefabName) return prefab;
        }

        return null;
    }
}
