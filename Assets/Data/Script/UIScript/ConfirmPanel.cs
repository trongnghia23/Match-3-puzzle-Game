using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ConfirmPanel : NghiaMono
{
    public Image[] Stars;
    public string LeveltoLoad;
    public int Level;
    protected override void Start()
    {
        base.Start();
        this.ActiveStar();
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
    protected virtual void ActiveStar()
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].enabled = false;
        }
    }
}
