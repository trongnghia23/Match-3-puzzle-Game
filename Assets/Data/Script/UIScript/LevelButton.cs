using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : NghiaMono
{
    [Header("Sprites")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite lockSprite;

    [Header("UI Refs")]
    [SerializeField] private Image[] starIcons;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private ConfirmPanel confirmPanel;

    private Image img;
    private Button btn;
    private int earnedStars;
    private bool isActive;
    [SerializeField] private int buttonLevel;

    /* ---------- Mono ---------- */
    protected override void Awake()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();

        // để chắc chắc GameData đã init
        StartCoroutine(WaitForGameDataThenInit());
    }

    private IEnumerator WaitForGameDataThenInit()
    {
        while (GameData.Instance == null) yield return null;
        Refresh();
        GameData.Instance.OnLoaded += Refresh;   // đăng ký lắng nghe khi GameData load lại
    }

    private void OnDisable()
    {
        if (GameData.Instance != null)
            GameData.Instance.OnLoaded -= Refresh;
    }

    /* ---------- Public ---------- */
    public void ShowConfirmPanel() => confirmPanel.Show(buttonLevel);

    /* ---------- Core ---------- */
    private void Refresh()
    {
        var data = GameData.Instance.savedata;
        isActive = data.IsActive[buttonLevel - 1];
        earnedStars = data.Stars[buttonLevel - 1];

        SetupSprite();
        SetupStars();
        levelText.text = buttonLevel.ToString();
    }

    private void SetupSprite()
    {
        img.sprite = isActive ? activeSprite : lockSprite;
        btn.enabled = isActive;
        levelText.enabled = isActive;
    }

    private void SetupStars()
    {
        if (starIcons == null) return;

        foreach (var s in starIcons) s.gameObject.SetActive(false);   // tắt hết
        for (int i = 0; i < Mathf.Min(earnedStars, starIcons.Length); i++)
            starIcons[i].gameObject.SetActive(true);                 // bật lại đúng số
    }
}

