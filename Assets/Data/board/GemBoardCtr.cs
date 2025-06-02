using UnityEngine;

public class GemBoardCtr : NghiaMono
{
    [SerializeField] protected Gemboard gemboard;
    public Gemboard Gemboard => gemboard;
   

    [SerializeField] protected Boardchecker boardchecker;
    public Boardchecker Boardchecker => boardchecker;

    [SerializeField] protected GemSwaper gemSwaper;    
    public GemSwaper GemSwaper => gemSwaper;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboard();
        this.LoadBoardchecker();
        this.LoadGemSwaper();
    }

    protected virtual void LoadGemboard()
    {
        if (this.gemboard != null) return;
        this.gemboard = GetComponent<Gemboard>();
        Debug.Log(transform.name + " :LoadGemboard", gameObject);
    }
    protected virtual void LoadBoardchecker()
    {
        if (this.boardchecker != null) return;
        this.boardchecker = GetComponentInChildren<Boardchecker>();
        Debug.Log(transform.name + " :LoadBoardchecker", gameObject);
    }

    protected virtual void LoadGemSwaper()
    {
        if (this.gemSwaper != null) return;
        this.gemSwaper = GetComponentInChildren<GemSwaper>();
        Debug.Log(transform.name + " :LoadGemSwaper", gameObject);
    }

    ///////////////////////////////
    

}