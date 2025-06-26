using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HintSystem : NghiaMono
{
    [SerializeField] private GemBoardCtr gemboardCtr;
    [SerializeField] private float hintDelay = 5f;

    private Vector2Int? hintSwapFrom;
    private Vector2Int? hintSwapTo;
    private Coroutine hintCoroutine;
    private FxCtr fxHintEffect;

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        if (gemboardCtr == null)
            gemboardCtr = FindAnyObjectByType<GemBoardCtr>();
    }

    public void StartHintTimer()
    {
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);
        hintCoroutine = StartCoroutine(HintTimer());
    }

    public void ResetHint()
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }
        ClearHint();
    }

    public void OnNoMatchFound()
    {
        ResetHint();
        StartHintTimer();
    }

    private IEnumerator HintTimer()
    {
        yield return new WaitForSeconds(hintDelay);

        if (FindHint(out Vector2Int from, out Vector2Int to))
        {
            hintSwapFrom = from;
            hintSwapTo = to;

            ShowHintFX(from);
            Debug.Log($"[Hint] Suggest swapping: {from} <-> {to}");
        }
        else
        {
            Debug.Log("[Hint] No hint found.");
        }
    }

    private void ShowHintFX(Vector2Int gridPos)
    {
        Vector3 worldPos = gemboardCtr.Gemboard.GetWorldPos(gridPos);
        string fxname = FxSpawer.Hint;

        if (fxHintEffect != null)
        {
            FxSpawer.Instance.Despawn(fxHintEffect.transform);
        }

        Transform fx = FxSpawer.Instance.Spawn(fxname, worldPos, Quaternion.identity);
        fxHintEffect = fx.GetComponent<FxCtr>();
    }

    public bool FindHint(out Vector2Int from, out Vector2Int to)
    {
        from = to = Vector2Int.zero;
        var board = gemboardCtr.Gemboard.gemBoardNode;

        for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
        {
            for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            {
                Node node = board[x, y];
                if (!CanUseHint(node)) continue;

                foreach (Vector2Int dir in new[] { Vector2Int.right, Vector2Int.up })
                {
                    int nx = x + dir.x;
                    int ny = y + dir.y;
                    if (!gemboardCtr.Gemboard.InBounds(nx, ny)) continue;

                    Node nei = board[nx, ny];
                    if (!CanUseHint(nei)) continue;

                    (node.Gem, nei.Gem) = (nei.Gem, node.Gem);
                    bool hasMatch = DeadLockChecker.Instance.HasMatchAt(x, y) || DeadLockChecker.Instance.HasMatchAt(nx, ny);
                    (node.Gem, nei.Gem) = (nei.Gem, node.Gem);

                    if (hasMatch)
                    {
                        from = new Vector2Int(x, y);
                        to = new Vector2Int(nx, ny);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CanUseHint(Node node)
    {
        return node != null && node.Gem != null && !node.IsBlocked();
    }

    public void ClearHint()
    {
        hintSwapFrom = hintSwapTo = null;
        if (fxHintEffect != null)
        {
            FxSpawer.Instance.Despawn(fxHintEffect.transform);
            fxHintEffect = null;
        }
    }
}
