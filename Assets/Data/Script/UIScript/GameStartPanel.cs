using UnityEngine;

public class GameStartPanel : NghiaMono
{
    public GameObject StartPanel;
    public GameObject LVSeclectPanel;
    protected override void Start()
    {
        base.Start();

        // Kiểm tra trạng thái lưu trước đó
        if (PlayerPrefs.HasKey("OpenLevelSelect") && PlayerPrefs.GetInt("OpenLevelSelect") == 1)
        {
            StartPanel.SetActive(false);
            LVSeclectPanel.SetActive(true);
        }
        else
        {
            StartPanel.SetActive(true);
            LVSeclectPanel.SetActive(false);
        }

        // Reset cờ để không giữ khi load scene lại lần nữa
        PlayerPrefs.DeleteKey("OpenLevelSelect");
    }
    public virtual void PlayGame()
    {
        StartPanel.SetActive(false);
        LVSeclectPanel.SetActive(true);
    }
    public virtual void HomeButton()
    {
        StartPanel.SetActive(true);
        LVSeclectPanel.SetActive(false);
    }
}
