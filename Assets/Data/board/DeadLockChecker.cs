using UnityEngine;

public class DeadLockChecker : NghiaMono
{
    protected static DeadLockChecker instance;
    public static DeadLockChecker Instance => instance;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    protected override void Awake()
    {
        base.Awake();
        if (DeadLockChecker.instance != null) Debug.LogError("only one DeadLockChecker allowed to exist");
        DeadLockChecker.instance = this;
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboardCtr();
    }
    protected virtual void LoadGemboardCtr()
    {
        this.gemboardCtr = GetComponentInParent<GemBoardCtr>();
    }

public bool IsDeadLock()
{
    for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
    {
        for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
        {
            if (gemboardCtr.Gemboard.arrayLayout.rows[y].row[x]) continue;

            // Chỉ thử swap với phải và dưới
            foreach (var offset in new (int dx, int dy)[] { (1, 0), (0, 1) })
            {
                int nx = x + offset.dx;
                int ny = y + offset.dy;

                if (nx >= gemboardCtr.Gemboard.width || ny >= gemboardCtr.Gemboard.height) continue;
                if (gemboardCtr.Gemboard.arrayLayout.rows[ny].row[nx]) continue;

                if (SwapAndCheckMatch(x, y, nx, ny))
                    return false; // Có match nếu swap
            }
        }
    }
    return true; // Không có move hợp lệ nào
}

private bool SwapAndCheckMatch(int x1, int y1, int x2, int y2)
{
    var node1 = gemboardCtr.Gemboard.gemBoardNode[x1, y1];
    var node2 = gemboardCtr.Gemboard.gemBoardNode[x2, y2];

    if (node1?.Gem == null || node2?.Gem == null) return false;

    // Swap tạm
    gemboardCtr.Gemboard.gemBoardNode[x1, y1] = node2;
    gemboardCtr.Gemboard.gemBoardNode[x2, y2] = node1;

    bool hasMatch = HasMatchAt(x1, y1) || HasMatchAt(x2, y2);

    // Swap lại
    gemboardCtr.Gemboard.gemBoardNode[x1, y1] = node1;
    gemboardCtr.Gemboard.gemBoardNode[x2, y2] = node2;

    return hasMatch;
}
private bool HasMatchAt(int x, int y)
{
    var gem = gemboardCtr.Gemboard.gemBoardNode[x, y]?.Gem;
    if (gem == null) return false;

    GemType type = gem.GetComponent<GemCtr>().GemType;

    // Horizontal
    int count = 1;
    for (int dx = -1; dx >= -2; dx--)
    {
        int nx = x + dx;
        if (nx >= 0 && gemboardCtr.Gemboard.gemBoardNode[nx, y]?.Gem?.GetComponent<GemCtr>().GemType == type)
            count++;
        else break;
    }
    for (int dx = 1; dx <= 2; dx++)
    {
        int nx = x + dx;
        if (nx < gemboardCtr.Gemboard.width && gemboardCtr.Gemboard.gemBoardNode[nx, y]?.Gem?.GetComponent<GemCtr>().GemType == type)
            count++;
        else break;
    }
    if (count >= 3) return true;

    // Vertical
    count = 1;
    for (int dy = -1; dy >= -2; dy--)
    {
        int ny = y + dy;
        if (ny >= 0 && gemboardCtr.Gemboard.gemBoardNode[x, ny]?.Gem?.GetComponent<GemCtr>().GemType == type)
            count++;
        else break;
    }
    for (int dy = 1; dy <= 2; dy++)
    {
        int ny = y + dy;
        if (ny < gemboardCtr.Gemboard.height && gemboardCtr.Gemboard.gemBoardNode[x, ny]?.Gem?.GetComponent<GemCtr>().GemType == type)
            count++;
        else break;
    }

    return count >= 3;
}






    
}
