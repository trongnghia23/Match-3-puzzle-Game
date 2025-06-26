using UnityEngine;

public class SoundManager : NghiaMono
{
    protected static SoundManager instance;
    public static SoundManager Instance { get => instance; }
    public AudioSource Despawnnoise;
    public AudioSource BackgroundMusic;
    public AudioSource Movenoise;
    protected override void Awake()
    {
        base.Awake();
        if (SoundManager.instance != null) Debug.LogError("only one SoundManager allow to exist");
        SoundManager.instance = this;
    }
    public void SetSoundEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("Sound", enabled ? 1 : 0);
        PlayerPrefs.Save();

        RefreshSoundState();
    }
    private bool IsSoundEnabled()
    {
        return !PlayerPrefs.HasKey("Sound") || PlayerPrefs.GetInt("Sound") != 0;
    }
    protected override void Start()
    {
        base.Start();
        this.RefreshSoundState();
    }
    public virtual void PlayBackGroundMusic()
    {
        if (!IsSoundEnabled()) return;
        BackgroundMusic.Play();
    }
    public virtual void PlayDespawNoise()
    {
        if (!IsSoundEnabled()) return;
        Despawnnoise.Play();
    }
    public virtual void PlayMoveNoise()
    {
        if (!IsSoundEnabled()) return;
        Movenoise.Play();
    }
    public void RefreshSoundState()
    {
        bool enabled = IsSoundEnabled();

        // Nhạc nền
        if (!enabled && BackgroundMusic.isPlaying)
            BackgroundMusic.Stop();
        else if (enabled && !BackgroundMusic.isPlaying)
            BackgroundMusic.Play();

        // Bạn cũng có thể mute thay vì stop:
        // BackgroundMusic.mute = !enabled;

        // Hiệu ứng (mute toàn bộ AudioListener để “tắt nhanh”)
        AudioListener.pause = !enabled;
    }
}
