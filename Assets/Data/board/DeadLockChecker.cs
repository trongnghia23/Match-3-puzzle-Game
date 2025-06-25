using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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

    public bool IsDeadLock(bool spawnTest = false)
    {
        for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
        {
            for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            {
                Node node = gemboardCtr.Gemboard.gemBoardNode[x, y];
                if (!AllowDeadLockCheck(node, spawnTest)) continue;

                foreach (var (dx, dy) in new (int, int)[] { (1, 0), (0, 1) })
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= gemboardCtr.Gemboard.width || ny >= gemboardCtr.Gemboard.height) continue;

                    Node nei = gemboardCtr.Gemboard.gemBoardNode[nx, ny];
                    if (!AllowDeadLockCheck(nei, spawnTest)) continue;

                    if (SwapAndCheckMatch(x, y, nx, ny, spawnTest))

                        return false;           // có nước đi
                }
            }
        }
        return true;                            // dead-lock
    }

    private bool SwapAndCheckMatch(int x1, int y1, int x2, int y2, bool spawnTest = false)
    {
        Node n1 = gemboardCtr.Gemboard.gemBoardNode[x1, y1];
        Node n2 = gemboardCtr.Gemboard.gemBoardNode[x2, y2];

        if (spawnTest)
        {
            if (!AllowDeadLockCheck(n1, spawnTest) || !AllowDeadLockCheck(n2, spawnTest))
                return false;
        }
        else
        {
            if (!CanSwap(n1) || !CanSwap(n2))
                return false;
        }

        (n1.Gem, n2.Gem) = (n2.Gem, n1.Gem);

        bool hasMatch = HasMatchAt(x1, y1, spawnTest) || HasMatchAt(x2, y2, spawnTest);

        (n1.Gem, n2.Gem) = (n2.Gem, n1.Gem);

        return hasMatch;
    }
    private bool InRange(int x, int y)
    {
        return x >= 0 && x < gemboardCtr.Gemboard.width &&
               y >= 0 && y < gemboardCtr.Gemboard.height;
    }

    private Node GetNodeSafe(int x, int y)
    {
        if (!InRange(x, y)) return null;
        return gemboardCtr.Gemboard.gemBoardNode[x, y];
    }
    private bool SameType(int x, int y, GemType type, bool spawnTest)
    {
        Node n = GetNodeSafe(x, y);
        if (n == null || n.Gem == null) return false;

        if (!AllowDeadLockCheck(n, spawnTest)) return false;

        return n.Gem.GetComponent<GemCtr>().GemType == type;
    }

    private bool HasMatchAt(int x, int y, bool spawnTest = false)
    {
        if (!InRange(x, y)) return false;

        Node center = gemboardCtr.Gemboard.gemBoardNode[x, y];
        if (!AllowDeadLockCheck(center, spawnTest)) return false;

        GemType t = center.Gem.GetComponent<GemCtr>().GemType;

        // ---- Horizontal ----
        int count = 1;
        for (int dx = -1; dx >= -2; dx--)
        {
            int nx = x + dx;
            if (!InRange(nx, y)) break;

            Node n = gemboardCtr.Gemboard.gemBoardNode[nx, y];
            if (!AllowDeadLockCheck(n, spawnTest)) break;

            if (SameType(nx, y, t, spawnTest)) count++;
            else break;
        }
        for (int dx = 1; dx <= 2; dx++)
        {
            int nx = x + dx;
            if (!InRange(nx, y)) break;

            Node n = gemboardCtr.Gemboard.gemBoardNode[nx, y];
            if (!AllowDeadLockCheck(n, spawnTest)) break;

            if (SameType(nx, y, t, spawnTest)) count++;
            else break;
        }
        if (count >= 3) return true;

        // ---- Vertical ----
        count = 1;
        for (int dy = -1; dy >= -2; dy--)
        {
            int ny = y + dy;
            if (!InRange(x, ny)) break;

            Node n = gemboardCtr.Gemboard.gemBoardNode[x, ny];
            if (!AllowDeadLockCheck(n, spawnTest)) break;

            if (SameType(x, ny, t, spawnTest)) count++;
            else break;
        }
        for (int dy = 1; dy <= 2; dy++)
        {
            int ny = y + dy;
            if (!InRange(x, ny)) break;

            Node n = gemboardCtr.Gemboard.gemBoardNode[x, ny];
            if (!AllowDeadLockCheck(n, spawnTest)) break;

            if (SameType(x, ny, t, spawnTest)) count++;
            else break;
        }

        return count >= 3;
    }




    public IEnumerator ShuffleBoard()
    {
        if (gemboardCtr.Gemboard.isShuffling) yield break;
        gemboardCtr.Gemboard.isShuffling = true;
        gemboardCtr.GemSwaper.isProccessingMove = true;

        const int MAX_ATTEMPT = 30;     // số lần thử tối đa
        int attempt = 0;
        bool success = false;

        /* ─── vòng lặp shuffle có kiểm tra ─── */
        while (attempt++ < MAX_ATTEMPT)
        {
            ShuffleMovableGems();                 // xáo Fisher–Yates
            yield return StartCoroutine(WaitUntilAllMoved());

            bool hasMatch = gemboardCtr.Boardchecker.checkBoard();
            bool deadLocked = IsDeadLock();       // gameplay ⇒ spawnTest = false

            if (!hasMatch && !deadLocked)         // ✅ bàn hợp lệ
            {
                success = true;
                break;
            }
        }

        /* ─── nếu vẫn thất bại sau MAX_ATTEMPT ─── */
        if (!success)
        {
            Debug.LogWarning($"Shuffle vẫn dead-lock sau {MAX_ATTEMPT} lần – force 1 nước đi");
            ForceCreateOneMove();                 // đảm bảo có nước đi
            yield return StartCoroutine(WaitUntilAllMoved());
        }

        gemboardCtr.GemSwaper.isProccessingMove = false;
        gemboardCtr.Gemboard.isShuffling = false;
    }

    /*----------------------------------------------------------
     *  Xáo Fisher–Yates chỉ trên các ô không bị tile block
     *---------------------------------------------------------*/
    private void ShuffleMovableGems()
    {
        List<Vector2Int> slots = new();
        List<GameObject> gems = new();

        for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
            for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            {
                if (gemboardCtr.Gemboard.arrayLayout.rows[y].row[x]) continue;

                Node n = gemboardCtr.Gemboard.gemBoardNode[x, y];
                if (n == null || n.IsBlocked() || n.Gem == null) continue;

                slots.Add(new Vector2Int(x, y));
                gems.Add(n.Gem);
            }

        /* Fisher–Yates */
        System.Random rng = new();
        for (int i = gems.Count - 1; i > 0; i--)
        {
            int k = rng.Next(i + 1);
            (gems[i], gems[k]) = (gems[k], gems[i]);
        }

        /* Áp lại vào board */
        for (int i = 0; i < slots.Count; i++)
        {
            int x = slots[i].x, y = slots[i].y;
            GameObject gObj = gems[i];

            gemboardCtr.Gemboard.gemBoardNode[x, y].Gem = gObj;

            var gem = gObj.GetComponent<GemCtr>();
            gem.SetIndicies(x, y);
            gem.GemMove.MoveToTarget(new Vector2(x - gemboardCtr.Gemboard.spacingX,
                                                 y - gemboardCtr.Gemboard.spacingY));
        }
    }

    /*----------------------------------------------------------
     *  Tạo cưỡng bức 1 nước đi (đổi màu 1 gem để có match)
     *---------------------------------------------------------*/
    private void ForceCreateOneMove()
    {
        // tìm 3 node liền kề ngang, không block, không tile
        for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            for (int x = 0; x < gemboardCtr.Gemboard.width - 2; x++)
            {
                Node a = gemboardCtr.Gemboard.gemBoardNode[x, y];
                Node b = gemboardCtr.Gemboard.gemBoardNode[x + 1, y];
                Node c = gemboardCtr.Gemboard.gemBoardNode[x + 2, y];

                if (a == null || b == null || c == null) continue;
                if (a.IsBlocked() || b.IsBlocked() || c.IsBlocked()) continue;

                GemType target = a.Gem.GetComponent<GemCtr>().GemType;
                b.Gem.GetComponent<GemCtr>().GemType = target;               // đổi màu B
                return;
            }
    }


    public IEnumerator WaitUntilAllMoved()
    {
        while (!GemSpawner.Instance.IsSpawnDone)
        {
            yield return null;
        }
    }


    // Kiểm tra node có được phép match (được tính trong deadlock, match check)
    private bool CanGemMatch(Node node)
    {
        if (node == null || node.Gem == null) return false;
        if (node.IsBlocked()) return false;

        // Chỉ cho phép match nếu node có tile LockTile hoặc không có tile
        if (node.Tile != null && node.Tile.tileType != TileType.LockTile)
            return false;

        return true;
    }


    // Kiểm tra node có được phép swap
    private bool CanSwap(Node node)
    {
        if (node == null || node.Gem == null) return false;
        if (node.IsBlocked()) return false;

        // KHÔNG cho swap nếu node có tile LockTile
        if (node.Tile != null && node.Tile.tileType == TileType.LockTile)
            return false;

        // Cho phép swap nếu không có tile
        if (node.Tile == null)
            return true;

        // Có tile khác LockTile => không swap
        return false;
    }

    // Sửa AllowDeadLockCheck để dùng CanGemMatch khi gameplay
    private bool AllowDeadLockCheck(Node node, bool spawnTest = false)
    {
        if (spawnTest)
        {
            return node != null && !node.IsBlocked();
        }
        else
        {
            return CanGemMatch(node);
        }
    }





}
