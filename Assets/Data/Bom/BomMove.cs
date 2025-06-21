using UnityEngine;
using System.Collections;

public class BomMove : NghiaMono
{
    [SerializeField] protected BombCtr BombCtr;
   

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadBombCtr();
    }
    protected virtual void LoadBombCtr()
    {
        if (this.BombCtr != null) return;
        this.BombCtr = transform.parent.GetComponent<BombCtr>();
        Debug.Log(transform.name + " :LoadBombCtr", gameObject);
    }
    public bool IsMoving; 
    

    public virtual void MoveToTarget(Vector2 targetpos)
    {
       GemSpawner.Instance.SpawnTimeUp();
        StartCoroutine(MoveCoroutine(targetpos));
    }

    protected virtual IEnumerator MoveCoroutine(Vector2 targetpos)
    {
        IsMoving = true;
        float time = 0.2f;

        Vector2 startPos = BombCtr.transform.position;
        float elaspedTime = 0f;
        while (elaspedTime < time)
        {
            float t = elaspedTime / time;
            BombCtr.transform.position = Vector2.Lerp(startPos, targetpos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        BombCtr.transform.position = targetpos;
        IsMoving = false;
        GemSpawner.Instance.SpawnTimeDown();
    }
    
}
