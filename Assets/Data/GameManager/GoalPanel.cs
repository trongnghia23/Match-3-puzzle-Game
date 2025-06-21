using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : NghiaMono
{
    public Image thisImage;
    public Sprite thisSprite;
    public TMP_Text thisText;
    public string thisString;

    protected override void Start()
    {
        base.Start();
        this.Setup();
    }

    protected void Setup()
    {
        this.thisImage.sprite = thisSprite;
        this.thisText.text = thisString;

    }
}
