using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuScenes : NghiaMono
{
    private GameData gameData;
    private GameManagerCtr gameManagerCtr;
    public string LeveltoLoad;
    protected override void Start()
    {
        base.Start();
        this.LoadgameData();
        this.LoadGameManagerCtr();
    } 
   
    protected virtual void LoadgameData()
    {
        if (this.gameData != null) return;
        this.gameData = Transform.FindAnyObjectByType<GameData>();
        Debug.Log(transform.name + " :LoadgameData", gameObject);
    }
    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = Transform.FindAnyObjectByType<GameManagerCtr>();
        Debug.Log(transform.name + " :LoadGameManagerCtr", gameObject);
    }
    public virtual void WinOK()
    {
        if (gameData != null)
        {
            int nextLevel = gameManagerCtr.GameManager.Level + 1;
if (nextLevel < gameData.savedata.IsActive.Length)
{
    gameData.savedata.IsActive[nextLevel] = true;
}
else
{
    Debug.Log("Đã hoàn thành level cuối cùng, không có level kế tiếp.");
}
            gameData.Save();
        }
        SceneManager.LoadScene(LeveltoLoad);
    }
    public virtual void LoseOK()
    {
        SceneManager.LoadScene(LeveltoLoad);
    }
}

