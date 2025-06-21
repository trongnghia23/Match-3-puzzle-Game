using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.LightTransport;
public class ScoreManager : NghiaMono
{
    public TMP_Text ScoreText;
    public int score;
    public int basePieceValue = 20;
    public int StreakValue = 0;
    public int[] ScoreGoals;
    public Image ScoreBar;
    protected virtual void Update()
    {
        ScoreText.text = "" + score;
    }
    public virtual void IncreaseScore(int scoreToIncrease)
    {
        score += scoreToIncrease;
        if (ScoreText != null && ScoreBar != null)
        {
            int length = ScoreGoals.Length;
            ScoreBar.fillAmount = (float)score / (float)ScoreGoals[length - 1];
        }
    }
}
