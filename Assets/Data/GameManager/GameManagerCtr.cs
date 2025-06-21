using UnityEngine;

public class GameManagerCtr : NghiaMono
{
    [SerializeField] protected GameManager gameManager;
    public GameManager GameManager => gameManager;
    [SerializeField] protected ScoreManager scoreManager;
    public ScoreManager ScoreManager => scoreManager;
    [SerializeField] protected SoundManager soundManager;
    public SoundManager SoundManager => soundManager;
    [SerializeField] protected GoalManager goalManager;
    public GoalManager GoalManager => goalManager;
    [SerializeField] protected GemBoardCtr gemBoardCtr;
    public GemBoardCtr GemBoardCtr => gemBoardCtr;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadscoreManager();
        this.LoadSoundManager();
        this.LoadGoalManager();
        this.LoadGameManager();
        this.LoadGemBoardCtr();
    }
    protected virtual void LoadGameManager()
    {
        if (this.gameManager != null) return;
        this.gameManager = GetComponent<GameManager>();
        Debug.Log(transform.name + " :LoadGameManager", gameObject);
    }
    protected virtual void LoadscoreManager()
    {
        if (this.scoreManager != null) return;
        this.scoreManager = GetComponentInChildren<ScoreManager>();
        Debug.Log(transform.name + " :LoadscoreManager", gameObject);
    }
    protected virtual void LoadSoundManager()
    {
        if (this.soundManager != null) return;
        this.soundManager = GetComponentInChildren<SoundManager>();
        Debug.Log(transform.name + " :LoadSoundManager", gameObject);
    }
    protected virtual void LoadGoalManager()
    {
        if (this.goalManager != null) return;
        this.goalManager = GetComponentInChildren<GoalManager>();
        Debug.Log(transform.name + " :LoadGoalManager", gameObject);
    }
    protected virtual void LoadGemBoardCtr()
    {
        if (this.gemBoardCtr != null) return;
        this.gemBoardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemBoardCtr", gameObject);
    }
   
}
