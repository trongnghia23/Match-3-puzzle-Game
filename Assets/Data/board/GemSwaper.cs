using System;
using UnityEngine;
using System.Collections;
public class GemSwaper : NghiaMono
{
   [SerializeField] protected GemCtr SelectedGem;
    
    [SerializeField] protected bool isProccessingMove;
    public bool IsProccessingMove => isProccessingMove;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] protected GemSpawnCtr gemSpawnCtr;
    
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
        if (isProccessingMove)
        {
            return;
        }
        if (this.SelectedGem == null)
        {
            Debug.Log(_gemCtrl.name);
            this.SelectedGem = _gemCtrl;
        }
        else if (this.SelectedGem == _gemCtrl)
        {
            this.SelectedGem = null;
        }
        else if (SelectedGem != _gemCtrl)
        {
            SwapGem(SelectedGem, _gemCtrl);
            this.SelectedGem = null;
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
        

        StartCoroutine(ProcessMatch(_currentGem, _targetGem));
    }
    protected virtual void Swap(GemCtr _currentGem, GemCtr _targetGem)
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
        }
        isProccessingMove = false;
        SelectedGem = null;
    }
    public virtual bool IsAbleToSwap(GemCtr _currentGem, GemCtr _targetGem)
    {
        return Mathf.Abs(_currentGem.xIndex - _targetGem.xIndex) + Mathf.Abs(_currentGem.yIndex - _targetGem.yIndex) == 1;
    } 
}
