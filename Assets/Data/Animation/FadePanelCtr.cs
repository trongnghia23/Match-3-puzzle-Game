using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class FadePanelCtr : NghiaMono
{
    public Animator PanelAnim;
    public Animator GameInfoAmim;
    public GameObject LoadAnim;
    public void Loading()
    {
        GameInfoAmim.SetBool("In", true);
        LoadAnim.SetActive(false);
    }
    public void Ok()
    {
        if (PanelAnim != null && GameInfoAmim != null)
        {
            PanelAnim.SetBool("Out", true);
            GameInfoAmim.SetBool("Out", true);
            PanelAnim.SetBool("GameOver", false);
            StartCoroutine(GameStart());
        }
    }
    public void GameOver()
    {
        PanelAnim.SetBool("Out", false);
        PanelAnim.SetBool("GameOver", true);
    }
    public IEnumerator GameStart()
    {
        yield return new WaitForSeconds(2f);
        GemBoardCtr gemBoardCtr = FindAnyObjectByType<GemBoardCtr>();
        if (gemBoardCtr == null)
        {
            Debug.LogError("Không tìm thấy GemBoardCtr!");
            yield break;
        }
        gemBoardCtr.SetGameState(GemBoardCtr.GameState.Move);
    }

}
