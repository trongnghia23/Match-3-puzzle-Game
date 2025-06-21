using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public enum GameType
{
    Time,
    Move
}
[Serializable]
public class EndGameType
{
    public GameType Gametype;
    public int CounterValue;
}
public class GameManager : NghiaMono
{
    [SerializeField] protected GameManagerCtr gameManagerCtr;
    public EndGameType endGameType;
    public GameObject MoveLabel;
    public GameObject TimeLabel;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public TMP_Text Counter;
    public int CurrenCounterValue;
    protected float timerSeconds = 1f;
    [Header("SO")]
    public TheWorld World;
    public int Level;
    protected override void Awake()
    {
        base.Awake();
        this.SelectLv();
        this.LoadLv();
    }
    protected override void Start()
    {
        base.Start();
        this.SetupGame();
        
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGameManagerCtr();
    }
    protected virtual void Update()
    {
        if (endGameType.Gametype == GameType.Time && CurrenCounterValue > 0)
        {
            this.TimeCounterValue();
        }
    }
    protected virtual void SelectLv()
    {
        if(PlayerPrefs.HasKey("CurrentLevel"))
        {
            Level = PlayerPrefs.GetInt("CurrentLevel");
        }
    }
    protected virtual void LoadLv()
    {
        if (World != null)
        {
            if (World.levels[Level] != null && Level < World.levels.Length)
            {
                endGameType.Gametype = World.levels[Level].Gametype.Gametype;
                endGameType.CounterValue = World.levels[Level].Gametype.CounterValue;
                gameManagerCtr.ScoreManager.ScoreGoals = World.levels[Level].ScoreGoals;
                gameManagerCtr.GemBoardCtr.Gemboard.height = World.levels[Level].height;
                gameManagerCtr.GemBoardCtr.Gemboard.width = World.levels[Level].width;
                gameManagerCtr.GemBoardCtr.Gemboard.arrayLayout = World.levels[Level].gemBoardLayout;
                gameManagerCtr.GoalManager.Goal = World.levels[Level].LevelGoals;
            }
        }
    }
    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = GetComponent<GameManagerCtr>();
        Debug.Log(transform.name + " :LoadGameManagerCtr", gameObject);
    }
    protected virtual void SetupGame()
    {
        CurrenCounterValue = endGameType.CounterValue;
        if (endGameType.Gametype == GameType.Move)
        {
            MoveLabel.SetActive(true);
            TimeLabel.SetActive(false);
        }
        else
        {
            MoveLabel.SetActive(false);
            TimeLabel.SetActive(true);
        }
        Counter.text = "" + CurrenCounterValue;
    }
    public virtual void DecreaseCounterValue()
    {
        if (gameManagerCtr.GemBoardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            CurrenCounterValue--;
            Counter.text = "" + CurrenCounterValue;
            if (CurrenCounterValue == 0)
            {
                this.LoseGame();
            }
        }
    }

    public virtual void WinGame()
    {
        WinPanel.SetActive(true);
        gameManagerCtr.GemBoardCtr.SetGameState(GemBoardCtr.GameState.Win);
        CurrenCounterValue = 0;
        Counter.text = "" + CurrenCounterValue;
        FadePanelCtr fade = FindAnyObjectByType<FadePanelCtr>();
        fade.GameOver();
    }
    public virtual void LoseGame()
    {
        LosePanel.SetActive(true);
        gameManagerCtr.GemBoardCtr.SetGameState(GemBoardCtr.GameState.Lose);
        CurrenCounterValue = 0;
        Debug.LogWarning("YouLose");
        Counter.text = "" + CurrenCounterValue;
        FadePanelCtr fade = FindAnyObjectByType<FadePanelCtr>();
        fade.GameOver();
    }
    protected virtual void TimeCounterValue()
    {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                this.DecreaseCounterValue();
                timerSeconds = 1;
            }
    }


}
