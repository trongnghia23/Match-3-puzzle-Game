using UnityEngine;
using System.Collections;

public class GemMove : NghiaMono
{
    [SerializeField] protected GemCtr gemCtr;
   

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemCtr();
    }
    protected virtual void LoadGemCtr()
    {
        if (this.gemCtr != null) return;
        this.gemCtr = transform.parent.GetComponent<GemCtr>();
        Debug.Log(transform.name + " :LoadGemCtr", gameObject);
    }
/*    public bool IsMoving; 
    

    public virtual void MoveToTarget(Vector2 targetpos)
    {
       
        StartCoroutine(MoveCoroutine(targetpos));
    }

    protected IEnumerator MoveCoroutine(Vector2 targetpos)
    {
        IsMoving = true;
        float time = 0.2f;

        Vector2 startPos = gemCtr.transform.position;
        float elaspedTime = 0f;
        while (elaspedTime < time)
        {
            float t = elaspedTime / time;
            gemCtr.transform.position = Vector2.Lerp(startPos, targetpos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        gemCtr.transform.position = targetpos;
        IsMoving = false;
    }
    */
}
