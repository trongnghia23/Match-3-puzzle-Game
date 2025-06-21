using UnityEngine;

public class GemBoardCtr : NghiaMono
{
    [SerializeField] protected Gemboard gemboard;
    public Gemboard Gemboard => gemboard;
   

    [SerializeField] protected Boardchecker boardchecker;
    public Boardchecker Boardchecker => boardchecker;

    [SerializeField] protected GemSwaper gemSwaper;    
    public GemSwaper GemSwaper => gemSwaper;

    [SerializeField] protected DeadLockChecker deadLockChecker;
    public DeadLockChecker DeadLockChecker => deadLockChecker;

    [SerializeField] protected GameManagerCtr gameManagerCtr;
    public GameManagerCtr GameManagerCtr => gameManagerCtr;

    public Vector2Int LastSwapPos;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboard();
        this.LoadBoardchecker();
        this.LoadGemSwaper();
        this.LoadDeadLockChecker();
      this.LoadGameManagerCtr();
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

    protected virtual void LoadDeadLockChecker()
    {
        if (this.deadLockChecker != null) return;
        this.deadLockChecker = GetComponentInChildren<DeadLockChecker>();
        Debug.Log(transform.name + " :LoadDeadLockChecker", gameObject);
    }
    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = Transform.FindAnyObjectByType<GameManagerCtr>();
        Debug.Log(transform.name + " :LoadGameManagerCtr", gameObject);
    }


    public GameState CurrentState = GameState.Move;
    public void SetGameState(GameState state)
    {
        Debug.Log($"State chuyển từ {CurrentState} => {state}");
        CurrentState = state;

    }
    public enum GameState
    {
        Wait,
        Win,
        Lose,
        Pause,
        Move,
    }
}