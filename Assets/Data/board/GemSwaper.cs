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

        Node node = gemboardCtr.Gemboard.gemBoardNode[_gemCtrl.xIndex, _gemCtrl.yIndex];
        if (node.Tile != null && node.Tile.TilesHp > 0)
        {
            Debug.Log("Tile chưa bị phá, không thể chọn gem.");
            return;
        }

        if (this.SelectedGem == null && !isProccessingMove && gemboardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
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
            // Kiểm tra luôn targetGem trước khi swap
            Node targetNode = gemboardCtr.Gemboard.gemBoardNode[_gemCtrl.xIndex, _gemCtrl.yIndex];
            if (targetNode.Tile != null && targetNode.Tile.TilesHp > 0)
            {
                Debug.Log("Tile tại vị trí swap chưa bị phá, không thể swap.");
                return;
            }

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
        Node nodeA = gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex];
        Node nodeB = gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex];
        if ((nodeA.Tile != null && nodeA.Tile.TilesHp > 0) ||
       (nodeB.Tile != null && nodeB.Tile.TilesHp > 0))
        {
            Debug.Log("Không thể swap vì tile chưa bị phá.");
            return;
        }
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
    protected virtual void Swap(GemCtr _currentGem, GemCtr _targetGem)
    {
        Vector2 targetPos = Gemboard.Instance.GetWorldPos(new Vector2Int(_targetGem.xIndex, _targetGem.yIndex));
        Vector2 currentPos = Gemboard.Instance.GetWorldPos(new Vector2Int(_currentGem.xIndex, _currentGem.yIndex));

        GameObject tempGem = gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex].Gem;
        gemboardCtr.Gemboard.gemBoardNode[_currentGem.xIndex, _currentGem.yIndex].Gem = gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex].Gem;
        gemboardCtr.Gemboard.gemBoardNode[_targetGem.xIndex, _targetGem.yIndex].Gem = tempGem;

        // Swap indices
        int tempX = _currentGem.xIndex;
        int tempY = _currentGem.yIndex;
        _currentGem.xIndex = _targetGem.xIndex;
        _currentGem.yIndex = _targetGem.yIndex;
        _targetGem.xIndex = tempX;
        _targetGem.yIndex = tempY;

        // Move to correct destination (dựa trên vị trí được tính trước)
        _currentGem.GemMove.MoveToTarget(targetPos);
        _targetGem.GemMove.MoveToTarget(currentPos);
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
