using UnityEngine;
using System.Collections;

public class BomMove : NghiaMono
{
    [SerializeField] protected BomCtr bomCtr;
   

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemCtr();
    }
    protected virtual void LoadGemCtr()
    {
        if (this.bomCtr != null) return;
        this.bomCtr = transform.parent.GetComponent<BomCtr>();
       // Debug.Log(transform.name + " :LoadGemCtr", gameObject);
    }
    public bool IsMoving; 
    

    public virtual void MoveToTarget(Vector2 targetpos)
    {
       
        StartCoroutine(MoveCoroutine(targetpos));
    }

    protected IEnumerator MoveCoroutine(Vector2 targetpos)
    {
        IsMoving = true;
        float time = 0.2f;

        Vector2 startPos = bomCtr.transform.position;
        float elaspedTime = 0f;
        while (elaspedTime < time)
        {
            float t = elaspedTime / time;
            bomCtr.transform.position = Vector2.Lerp(startPos, targetpos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        bomCtr.transform.position = targetpos;
        IsMoving = false;
    }
    
}
