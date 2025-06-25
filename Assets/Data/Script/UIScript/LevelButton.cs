using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : NghiaMono
{
    [Header("Active")]
    public bool isActive;
    public Sprite ActiveSprite;
    public Sprite LockSprite;
    protected Image ButtonImage;
    protected Button MyButton;
    private int StarsActive;
    [Header("LvUI")]
    public Image[] Stars;
    public int ButtonLevel;
    public TMP_Text LevelText;
    public ConfirmPanel ConfirmPanel;
   

    private GameData gameData;
    
    protected override void Start()
    {
        base.Start();
        ButtonImage = GetComponent<Image>();
        MyButton = GetComponent<Button>();
        UpdateUI();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(DelayedLoadData());
    }

    private IEnumerator DelayedLoadData()
    {
        yield return null; // đợi 1 frame
        UpdateUI(); // LoadData, DecideSprite, ...
    }
    protected virtual void UpdateUI()
    {
        LoadData();
        DecideSprite();
        ShowLv();
        ActiveStar();
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGameData();
        this.LoadConfirmPanel();
    }
    protected virtual void LoadGameData()
    {
        if (this.gameData != null) return;
        this.gameData = Transform.FindAnyObjectByType<GameData>();
        Debug.Log(transform.name + " :LoadGameData", gameObject);
    }

    public virtual void LoadData()
    {
        if (gameData == null) return;

        isActive = gameData.savedata.IsActive[ButtonLevel - 1];
        StarsActive = gameData.savedata.Stars[ButtonLevel - 1];
    }
    public virtual void DecideSprite()
    {
        if (isActive)
        {
            ButtonImage.sprite = ActiveSprite;
            MyButton.enabled = true;
            LevelText.enabled = true;
        }
        else 
        {
            ButtonImage.sprite = LockSprite;
            MyButton.enabled = false;
            LevelText.enabled = false;
        }
    }
    protected virtual void ActiveStar()
    {
        int count = Mathf.Min(StarsActive, Stars.Length);
        for (int i = 0; i < count; i++)
        {
            Stars[i].gameObject.SetActive(true);
        }
    }
    protected virtual void ShowLv()
    {
        LevelText.text = "" + ButtonLevel;
    }
    protected virtual void LoadConfirmPanel()
    {
        if (this.ConfirmPanel != null) return;
        this.ConfirmPanel = Transform.FindAnyObjectByType<ConfirmPanel>();
        Debug.Log(transform.name + " :LoadConfirmPanel", gameObject);
    }
    public virtual void ShowConfirmPanel()
    {
        ConfirmPanel.Level = ButtonLevel;
        ConfirmPanel.gameObject.SetActive(true);
    }
    
}

