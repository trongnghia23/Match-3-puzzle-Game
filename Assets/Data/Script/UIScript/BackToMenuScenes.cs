using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuScenes : NghiaMono
{
    public string LeveltoLoad;
    public virtual void OK()
    {
        SceneManager.LoadScene(LeveltoLoad);
    }
}
