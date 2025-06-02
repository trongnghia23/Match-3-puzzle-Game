using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
[RequireComponent(typeof(BoxCollider2D))]
public class GemCtr : NghiaMono
{
   // protected static GemCtr instance;
   // public static GemCtr Instance => instance;
    [SerializeField] protected BoxCollider2D _collider;
    [SerializeField] protected GemDespawn gemDespawn;
    public GemDespawn GemDespawn => gemDespawn;
    [SerializeField] protected GemMove gemMove;
    public GemMove GemMove => gemMove;

    public GemType GemType;
    public int xIndex;
    public int yIndex;
    public bool ItMatched;
    protected Vector2 currentpos;
    protected Vector2 targetpos;
    //public bool IsMoving;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadTrigger();
        this.LoadGemDespawn();
        this.LoadGemMove();
    }
    protected virtual void LoadTrigger()
    {
        if (this._collider != null) return;
        this._collider = transform.GetComponent<BoxCollider2D>();
        this._collider.isTrigger = false;
        this._collider.size = new Vector2(0.8f, 0.8f);
        Debug.LogWarning(transform.name + " LoadTrigger", gameObject);
    }
    protected virtual void LoadGemDespawn()
    {
        if (this.gemDespawn != null) return;
        this.gemDespawn = transform.GetComponentInChildren<GemDespawn>();
        Debug.Log(transform.name + " :LoadGemDespawn", gameObject);
    }
    protected virtual void LoadGemMove()
    {
        if (this.gemMove != null) return;
        this.gemMove = GetComponentInChildren<GemMove>();
        Debug.Log(transform.name + " :LoadGemMove", gameObject);
    }
    public GemCtr(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
   public void SetIndicies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }
///////////////////////////////
public bool IsMoving; 
    

    public virtual void MoveToTarget(Vector2 _targetpos)
    {
       
        StartCoroutine(MoveCoroutine(_targetpos));
    }
    protected IEnumerator MoveCoroutine(Vector2 _targetpos)
    {
        IsMoving = true;
        float time = 0.2f;

        Vector2 startPos = transform.position;
        float elaspedTime = 0f;
        while (elaspedTime < time)
        {
            float t = elaspedTime / time;
            transform.position = Vector2.Lerp(startPos, _targetpos, t);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = _targetpos;
        IsMoving = false;
    }
}

public enum GemType
{
    Blue,
    Red,
    Green,
    Yellow,
    Orange
} 