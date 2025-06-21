using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : NghiaMono
{
    [Header("Active")]
    public bool isActive;
    public Sprite ActiveSprite;
    public Sprite LockSprite;
    protected Image ButtonImage;
    protected Button MyButton;
    public TMP_Text LevelText;

    public Image[] Stars;
    public int ButtonLevel;
    public ConfirmPanel ConfirmPanel;
    protected override void Start()
    {
        base.Start();
        ButtonImage = GetComponent<Image>();
        MyButton = GetComponent<Button>();
        this.DecideSprire();
        this.ShowLv();
    } 
    public virtual void DecideSprire()
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
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].enabled = false;
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
        ConfirmPanel.GetComponent<ConfirmPanel>().Level = ButtonLevel;
        this.ConfirmPanel.gameObject.SetActive(true);
    }
    
}

