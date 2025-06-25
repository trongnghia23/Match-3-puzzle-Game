using System.Security.Cryptography;
using UnityEngine;

public class TileDespawn : DespawnTileByCondition
{
    [SerializeField]
    private DespawnConditionType activeConditions =
            DespawnConditionType.HPZero;
    public override bool CanDespawn()
    {
        if (activeConditions.HasFlag(DespawnConditionType.HPZero) && IsTileDead()) return true;
        if (activeConditions.HasFlag(DespawnConditionType.Move) && IsMoveLimitReached()) return true;
        if (activeConditions.HasFlag(DespawnConditionType.TimeBased) && IsTimeLimitReached()) return true;
        return false;
    }
    public override void DespawnObject()
    {
        TileCtr tile = transform.parent.GetComponent<TileCtr>();
        if (tile == null) return;

        tile.TilesHp = 0;

        if (tile.node != null)
        {
            tile.node.Tile = null;
            tile.SetNode(null, tile.xIndex, tile.yIndex);
        }
        FxSpawer.Instance.Despawn(transform.parent);
    }
    
}
[System.Flags]
public enum DespawnConditionType
{
    None = 0,
    HPZero = 1 << 0,
    Move = 1 << 1,
    TimeBased = 1 << 2
}