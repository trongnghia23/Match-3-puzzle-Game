using UnityEngine;

public class LevelSelectManager : NghiaMono
{
    public GameObject[] Panels;
    public GameObject CurrentPanel;
    public int page;
    private GameData gameData;
    private int currentLevel = 0;
    protected override void Start()
    {
        base.Start();

        gameData = GameData.Instance;

        // Tắt tất cả panel
        foreach (var panel in Panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        // Nếu GameData chưa sẵn sàng → mở trang đầu tiên
        if (gameData == null || gameData.savedata == null || gameData.savedata.IsActive == null)
        {
            Debug.LogWarning("[LevelSelectManager] GameData chưa sẵn sàng, mở trang đầu.");
            page = 0;
        }
        else
        {
            // Xác định level cao nhất đang mở
            for (int i = 0; i < gameData.savedata.IsActive.Length; i++)
            {
                if (gameData.savedata.IsActive[i])
                    currentLevel = i;
            }

            // Tính toán page từ level
            page = Mathf.Clamp(currentLevel / 9, 0, Panels.Length - 1);
        }

        // Mở panel tương ứng
        if (page >= 0 && page < Panels.Length)
        {
            CurrentPanel = Panels[page];
            CurrentPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[LevelSelectManager] Không tìm thấy panel hợp lệ.");
        }
    }
    public virtual void PageRight()
    {
        if (page < Panels.Length - 1)
        {
            if (CurrentPanel != null)
                CurrentPanel.SetActive(false);

            page++;
            CurrentPanel = Panels[page];
            CurrentPanel.SetActive(true);
        }
    }
    public virtual void PageLeft()
    {
        if (page > 0)
        {
            if (CurrentPanel != null)
                CurrentPanel.SetActive(false);

            page--;
            CurrentPanel = Panels[page];
            CurrentPanel.SetActive(true);
        }
    }
}
