using System;
using UnityEngine;
using System.Collections;
public class GemSwaper : NghiaMono
{
   [SerializeField] protected GemCtr SelectedGem;
    
    [SerializeField] public bool isProccessingMove;
   public bool IsProccessingMove => isProccessingMove;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] protected GemSpawnCtr gemSpawnCtr;

    private FxCtr fxChoseEffect;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboardCtr();
        this.LoadGemSpawnCtr();
    }
    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = GetComponentInParent<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemboardCtr", gameObject);
    }
    protected virtual void LoadGemSpawnCtr()
    {
        if (this.gemSpawnCtr != null) return;
        this.gemSpawnCtr = Transform.FindAnyObjectByType<GemSpawnCtr>();
        Debug.Log(transform.name + " :LoadGemSpawnCtr", gameObject);
    }
    public virtual void SelectGem(GemCtr _gemCtrl)
    {
        if (this.SelectedGem == null && !isProccessingMove && gemboardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            Debug.Log(_gemCtrl.name);
            this.SelectedGem = _gemCtrl;
            this.OnChose();
        }
        else if (this.SelectedGem == _gemCtrl && !isProccessingMove && gemboardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            this.DespawnFx();
            this.SelectedGem = null;
        }
        else if (SelectedGem != _gemCtrl && !isProccessingMove && gemboardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            SwapGem(SelectedGem, _gemCtrl);
            this.SelectedGem = null;
            this.DespawnFx();
        }

    }
    protected virtual void OnChose()
    {
        string fxname = this.GetChoseFxname();
        Vector3 Pos = SelectedGem.transform.position;
        Transform fxOnDead = FxSpawer.Instance.Spawn(fxname, Pos, transform.rotation);
        fxChoseEffect = fxOnDead.GetComponent<FxCtr>();
        fxOnDead.gameObject.SetActive(true);
    }
    protected virtual string GetChoseFxname()
    {
        return FxSpawer.Chose;
    }
    protected virtual void DespawnFx()
    {
        if (fxChoseEffect != null)
        {
            fxChoseEffect.FxDespawn.DespawnObject();
            fxChoseEffect = null;
        }
    }

    public virtual void SwapGem(GemCtr _currentGem, GemCtr _targetGem)
    {
        if (!IsAbleToSwap(_currentGem, _targetGem))
        {
            Debug.Log("Not able to swap");
            return;
        }
        Swap(_currentGem, _targetGem);
        isProccessingMove = true;
        
        if (gemboardCtr.GameManagerCtr.GameManager != null && gemboardCtr.GameManagerCtr.GameManager.endGameType.Gametype == GameType.Move)
        {
            gemboardCtr.GameManagerCtr.GameManager.DecreaseCounterValue();
        }
        

        StartCoroutine(ProcessMatch(_currentGem, _targetGem));
    }
    protected virtual void Swap(GemCtr _currentGem, GemCtr _targetGem )
    {
        Transform temp = gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex].Gem.transform;
        gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex].Gem = gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex].Gem.gameObject;
        gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex].Gem = temp.gameObject;

        int tempX = _currentGem.xIndex;
        int tempY = _currentGem.yIndex;
        _currentGem.xIndex = _targetGem.xIndex;
        _currentGem.yIndex = _targetGem.yIndex;
        _targetGem.xIndex = tempX;
        _targetGem.yIndex = tempY;

        _currentGem.GemMove.MoveToTarget(gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex].Gem.transform.position);
        _targetGem.GemMove.MoveToTarget(gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex].Gem.transform.position);
    }
    
    protected IEnumerator ProcessMatch(GemCtr _currentGem, GemCtr _targetGem)
    {
        yield return new WaitForSeconds(0.2f);
        if (gemboardCtr.Boardchecker.checkBoard())
        {
            StartCoroutine(gemboardCtr.Boardchecker.ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            Swap(_currentGem, _targetGem);
            isProccessingMove = false;
        }
        
        SelectedGem = null;

    }
    public virtual bool IsAbleToSwap(GemCtr _currentGem, GemCtr _targetGem)
    {
        return Mathf.Abs(_currentGem.xIndex - _targetGem.xIndex) + Mathf.Abs(_currentGem.yIndex - _targetGem.yIndex) == 1;
    } 
}
