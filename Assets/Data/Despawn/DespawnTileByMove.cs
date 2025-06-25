using UnityEngine;

public class DespawnTileByMove : Despawn
{
    [SerializeField] protected TileCtr tileCtr;
    [SerializeField] protected GameManagerCtr gameManagerCtr;
    [SerializeField] protected int moveThreshold = 0; 


    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadTileCtr();
        this.LoadGameManagerCtr();
    }

    protected virtual void LoadTileCtr()
    {
        if (this.tileCtr != null) return;
        this.tileCtr = GetComponent<TileCtr>();
    }

    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = FindAnyObjectByType<GameManagerCtr>();
    }

    public override bool CanDespawn()
    {
        return IsMoveLimitReached();
    }

    protected virtual bool IsMoveLimitReached()
    {
        if (gameManagerCtr == null || gameManagerCtr.GameManager == null) return false;
        return gameManagerCtr.GameManager.endGameType.Gametype == GameType.Move &&
               gameManagerCtr.GameManager.CurrenCounterValue <= moveThreshold;
    }

   
}
