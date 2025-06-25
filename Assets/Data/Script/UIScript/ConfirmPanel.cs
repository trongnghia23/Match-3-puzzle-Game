using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ConfirmPanel : NghiaMono
{
    [Header("LvUI")]
    public TMP_Text HighScoreText;
    public TMP_Text StarsText;
    public Image[] Stars;

    [Header("LvInfo")]
    private int StarsActive;
    public string LeveltoLoad;
    public int Level;
    private int HighScore;
    private GameData gameData;
    protected override void OnEnable()
    {
        base.OnEnable();
        this.LoadData();
        this.ActiveStar();
        this.SetText();
    } 
    
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGameData();
    }
    protected virtual void LoadGameData()
    {
        if (this.gameData != null) return;
        this.gameData = Transform.FindAnyObjectByType<GameData>();
        Debug.Log(transform.name + " :LoadGameData", gameObject);
    }
    public virtual void LoadData()
    {
        if (gameData != null)
        {
                StarsActive = gameData.savedata.Stars[Level - 1];
                HighScore = gameData.savedata.HighScores[Level - 1];
        }
    }
    protected virtual void SetText()
    {
        HighScoreText.text = "" + HighScore;
        StarsText.text = StarsActive + "/3";
    }
    protected virtual void ActiveStar()
    {
        for (int i = 0; i < StarsActive; i++)
        {
            Stars[i].gameObject.SetActive(true);
        }
    }
    public virtual void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Play()
    {
        PlayerPrefs.SetInt("CurrentLevel", Level - 1);
        SceneManager.LoadScene(LeveltoLoad);
    }
    
}

