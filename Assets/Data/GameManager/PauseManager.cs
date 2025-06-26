using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : NghiaMono
{
    public GameObject PausePanel;
    [SerializeField]protected GemBoardCtr gemboardCtr;
    public bool Pause = false;
    public Image SoundButton;
    public Sprite MusicOnSprite;
    public Sprite MusicOffSprite;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboardCtr();
    }
    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemboardCtr", gameObject);
    }
    protected override void Start()
    {
        base.Start();
        
        PausePanel.SetActive(false);
    }
    protected void Update()
    {
        if (Pause && !PausePanel.activeInHierarchy)
        {
            PausePanel.SetActive(true);
            gemboardCtr.SetGameState(GemBoardCtr.GameState.Pause);
        }
        if (!Pause && PausePanel.activeInHierarchy)
        {
            PausePanel.SetActive(false);
            gemboardCtr.SetGameState(GemBoardCtr.GameState.Move);
        }
    }
    public virtual void SoundButtonSet()
    {
        bool isSoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;

        // Đảo trạng thái
        bool newSoundState = !isSoundOn;

        // Lưu trạng thái mới
        PlayerPrefs.SetInt("Sound", newSoundState ? 1 : 0);
        PlayerPrefs.Save();

        // Cập nhật icon
        SoundButton.sprite = newSoundState ? MusicOnSprite : MusicOffSprite;

        // Cập nhật trạng thái âm thanh ngay lập tức
        SoundManager.Instance.RefreshSoundState();
    }
    public virtual void PauseGame()
    {
        Pause = !Pause;
    }
    public virtual void ExitGame()
    {
        PlayerPrefs.SetInt("OpenLevelSelect", 1);
        SceneManager.LoadScene("MenuScenes");
    }

}
