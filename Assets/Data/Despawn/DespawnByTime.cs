using UnityEngine;

public class DespawnByTime : Despawn
{
    [SerializeField] protected float delay = 2f;
    [SerializeField] protected float timer = 0;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    protected override void OnEnable()
    {
        base.OnEnable();
        this.ResetTimer();
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemBoardCtr();
    }
    protected virtual void LoadGemBoardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemBoardCtr", gameObject);
    }
    protected virtual void ResetTimer()
    {
        this.timer = 0;
    }

    public override bool CanDespawn()
    {
        if (gemboardCtr.CurrentState == GemBoardCtr.GameState.Move)
        {
            this.timer += Time.fixedDeltaTime;
            if (this.timer > this.delay) return true;
        }
        return false;
    }
}