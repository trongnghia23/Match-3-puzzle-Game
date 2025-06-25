using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : NghiaMono
{
    public TMP_Text ScoreText;
    public int score;
    public int basePieceValue = 20;
    public int StreakValue = 0;
    public int[] ScoreGoals;
    public Image ScoreBar;
    [SerializeField] protected GameData gameData;
    [SerializeField] protected GameManagerCtr gameManagerCtr;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadgameData();
        this.LoadGameManagerCtr();
    } 

    protected virtual void LoadgameData()
    {
        if (this.gameData != null) return;
        this.gameData = FindAnyObjectByType<GameData>();
        Debug.Log(transform.name + " :LoadgameData", gameObject);
    }
    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = GetComponentInParent<GameManagerCtr>();
        Debug.Log(transform.name + " :LoadGameManagerCtr", gameObject);
    }
    protected virtual void Update()
    {
        ScoreText.text = "" + score;
    }
    public virtual void IncreaseScore(int scoreToIncrease)
    {
        score += scoreToIncrease;
        if (gameData != null)
        {
            int HighScore = gameData.savedata.HighScores[gameManagerCtr.GameManager.Level];
            if (score > HighScore)
            {
                gameData.savedata.HighScores[gameManagerCtr.GameManager.Level] = score;
            }
            gameData.Save();
        }
        this.UpdateBar();
    }
    public virtual void UpdateBar()
    {
        if (ScoreText != null && ScoreBar != null)
        {
            int length = ScoreGoals.Length;
            ScoreBar.fillAmount = (float)score / (float)ScoreGoals[length - 1];
        }
    }
}

