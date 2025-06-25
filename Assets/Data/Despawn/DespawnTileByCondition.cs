using UnityEngine;

public class DespawnTileByCondition : Despawn
{
    [SerializeField] protected TileCtr tileCtr;
    [SerializeField] protected GameManagerCtr gameManagerCtr;
    [SerializeField] protected int moveThreshold = 0; // mặc định despawn khi move == 0
    [SerializeField] protected float timeThreshold = 0f; // mặc định despawn khi time == 0 (cho GameType.Time)
    private int startingCounterValue;
    private bool hasCapturedStartValue = false;
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadTileCtr();
        this.LoadGameManagerCtr();
    }

    protected virtual void LoadTileCtr()
    {
        if (this.tileCtr != null) return;
        this.tileCtr = GetComponentInParent<TileCtr>();
    }

    protected virtual void LoadGameManagerCtr()
    {
        if (this.gameManagerCtr != null) return;
        this.gameManagerCtr = FindAnyObjectByType<GameManagerCtr>();
    }

    public override bool CanDespawn()
    {
        return IsTileDead() || IsMoveLimitReached() || IsTimeLimitReached();
    }

    protected virtual bool IsTileDead()
    {
        return tileCtr != null && tileCtr.TilesHp <= 0;
    }

    protected virtual bool IsMoveLimitReached()
    {
        if (gameManagerCtr == null || gameManagerCtr.GameManager == null) return false;
        return gameManagerCtr.GameManager.endGameType.Gametype == GameType.Move &&
               gameManagerCtr.GameManager.CurrenCounterValue <= moveThreshold;
    }

    protected virtual bool IsTimeLimitReached()
    {
        if (gameManagerCtr == null || gameManagerCtr.GameManager == null) return false;

        var gm = gameManagerCtr.GameManager;

        if (gm.endGameType.Gametype != GameType.Time) return false;

        if (!hasCapturedStartValue)
        {
            startingCounterValue = gm.CurrenCounterValue;
            hasCapturedStartValue = true;
        }

        int decreaseCount = startingCounterValue - gm.CurrenCounterValue;

        return decreaseCount >= timeThreshold;
    }
}
