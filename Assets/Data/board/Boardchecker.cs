using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static GemBoardCtr;
public class Boardchecker : NghiaMono
{
    protected static Boardchecker instance;
    public static Boardchecker Instance => instance;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] protected GemSpawnCtr gemSpawnCtr;
    
    protected override void Awake()
    {
        base.Awake();
        if (Boardchecker.instance != null) Debug.LogError("only one Boardchecker allowed to exist");
        Boardchecker.instance = this;
    }

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
        this.gemSpawnCtr = UnityEngine.Object.FindAnyObjectByType<GemSpawnCtr>();
        Debug.Log(transform.name + " :LoadGemSpawnCtr", gameObject);
    }
    public bool checkBoard()
    {
        Debug.Log("checkBoard");
        bool hasMatch = false;
        gemSpawnCtr.GemSpawner.GemtoRemove.Clear();

        foreach (Node NodeGem in gemboardCtr.Gemboard.gemBoardNode)
        {
            if (NodeGem.Gem != null)
            {
                NodeGem.Gem.GetComponent<GemCtr>().ItMatched = false;
            }
        }

        for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
        {
            for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            {
                Node node = gemboardCtr.Gemboard.gemBoardNode[x, y];
                if (node.isUsable && node.Gem != null)
                {
                    GemCtr gem = node.Gem.GetComponent<GemCtr>();
                    if (gem != null && !gem.ItMatched)
                    {
                        MatchResult matched = IsConnected(gem);
                        if (matched.connectedgems.Count >= 3)
                        {
                            MatchResult superMatched = SuperMached(matched);

                            foreach (GemCtr g in superMatched.connectedgems)
                            {
                                if (!g.ItMatched)
                                {
                                    gemSpawnCtr.GemSpawner.GemtoRemove.Add(g);
                                    g.ItMatched = true;
                                }
                                
                            }
                           gemboardCtr.HintSystem.ResetHint();
                            hasMatch = true;
                        }
                    }
                }
            }
        }
        return hasMatch;
    }

    public void ResetState()
    {
        gemboardCtr.GameManagerCtr.ScoreManager.StreakValue = 0;
        gemboardCtr.GemSwaper.isProccessingMove = false;
    }
    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves)
    {
       
       
        gemboardCtr.GemSwaper.isProccessingMove = true;
        HandleTileHit(gemSpawnCtr.GemSpawner.GemtoRemove);
        bool hasShuffled = false;
        while (true)
        {
            yield return StartCoroutine(gemSpawnCtr.GemSpawner.RemoveAndRefillGem(gemSpawnCtr.GemSpawner.GemtoRemove));

            // chờ spawn xong
            while (!GemSpawner.Instance.IsSpawnDone) yield return null;
            yield return new WaitUntil(() =>
           gemboardCtr.CurrentState == GemBoardCtr.GameState.Move);
            yield return new WaitForSeconds(0.2f);

            if (gemboardCtr.Gemboard.isInitializingBoard || gemboardCtr.Gemboard.isShuffling)
                break;
            SpreadAllSlimes();
            if (checkBoard())          // còn match → loop lại
            {
                HandleTileHit(gemSpawnCtr.GemSpawner.GemtoRemove);
                continue;
            }
            if (DeadLockChecker.Instance.IsDeadLock())
            {
                yield return StartCoroutine(gemboardCtr.DeadLockChecker.ShuffleBoard());
                hasShuffled = true;
                break;
            }
            break;
        }
        if (!hasShuffled)
        {
            gemboardCtr.GameManagerCtr.ScoreManager.IncreaseScore(gemboardCtr.GameManagerCtr.ScoreManager.basePieceValue * gemboardCtr.GameManagerCtr.ScoreManager.StreakValue);
        }
       
        ResetState();
    }

    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
    new(0, 1), new(0, -1), new(1, 0), new(-1, 0)
    };

    private void HandleTileHit(List<GemCtr> gems)
    {
        HashSet<TileCtr> tilesToHit = new();

        foreach (var gem in gems)
        {
            gem.ItMatched = false;
            gemboardCtr.GameManagerCtr.ScoreManager.StreakValue++;

            var node = gemboardCtr.Gemboard.gemBoardNode[gem.xIndex, gem.yIndex];
            var tile = node.Tile;

            if (tile != null && tile.TilesHp > 0)
                tilesToHit.Add(tile);

            foreach (var dir in directions)
            {
                int nx = gem.xIndex + dir.x;
                int ny = gem.yIndex + dir.y;

                if (nx < 0 || ny < 0 ||
                    nx >= gemboardCtr.Gemboard.width || ny >= gemboardCtr.Gemboard.height)
                    continue;

                var neighborTile = gemboardCtr.Gemboard.gemBoardNode[nx, ny].Tile;
                if (neighborTile != null && neighborTile.TilesHp > 0 &&
                    neighborTile.tileType != TileType.LockTile)   // loại trừ LockTile cạnh bên
                {
                    tilesToHit.Add(neighborTile);
                }
            }
        }

        // Chỉ gọi Hit một lần cho mỗi tile
        foreach (var t in tilesToHit)
        {
            int oldHp = t.TilesHp;
            t.Hit();                       // trừ HP

            if (oldHp > 0 && t.TilesHp <= 0)   // tile vừa bị phá xong
            {
                gemboardCtr.GameManagerCtr.GoalManager.CompareGoal(t.tag); // hay t.TileType.ToString()
                gemboardCtr.GameManagerCtr.GoalManager.updateGoal();
            }
        }
    }
    public void SpreadAllSlimes()
    {
        int spreadCount = 0;
        foreach (Node n in gemboardCtr.Gemboard.gemBoardNode)
        {
            var t = n.Tile;
            if (t != null && t.tileType == TileType.SlimeTile && t.TilesHp > 0)
            {
                if (t.OnPlayerMove())           // chỉ true khi thực sự lan
                {
                    spreadCount++;
                    if (spreadCount >= 2) break; // tối đa 2 slime lan mỗi lượt
                }
            }
        }
    }
    protected MatchResult SuperMached(MatchResult _MachedResult)
    {
        List<GemCtr> combinedGems = new(_MachedResult.connectedgems);
        bool foundSuperMatch = false;

        
        if (_MachedResult.direction == MatchDirection.doc || _MachedResult.direction == MatchDirection.docdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> extraGems = new();
                checkDirection(gem, new Vector2Int(0, 1), extraGems);
                checkDirection(gem, new Vector2Int(0, -1), extraGems);

                if (extraGems.Count >= 3)
                {
                    Debug.LogWarning("an ngang sieu cap " + gem.GemType);
                    foreach (var g in extraGems)
                    {
                        if (!combinedGems.Contains(g))
                            combinedGems.Add(g);
                    }
                    foundSuperMatch = true;
                    break;
                }
            }
        }
        else if (_MachedResult.direction == MatchDirection.ngang || _MachedResult.direction == MatchDirection.ngangdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> extraGems = new();
                checkDirection(gem, new Vector2Int(1, 0), extraGems);
                checkDirection(gem, new Vector2Int(-1, 0), extraGems);

                if (extraGems.Count >= 3)
                {
                    Debug.LogWarning("an doc sieu cap " + gem.GemType);
                    foreach (var g in extraGems)
                    {
                        if (!combinedGems.Contains(g))
                            combinedGems.Add(g);
                    }
                    foundSuperMatch = true;
                    break;
                }
            }
        }

        return new MatchResult
        {
            connectedgems = combinedGems,
            direction = foundSuperMatch ? MatchDirection.super : _MachedResult.direction
        };

    }


    MatchResult IsConnected(GemCtr gem)
    {
        Dictionary<string, List<Vector2Int>> directionGroups = new()
    {
        { "vertical", new List<Vector2Int> { new(0, 1), new(0, -1) } },
        { "horizontal", new List<Vector2Int> { new(1, 0), new(-1, 0) } }
    };

        Dictionary<string, List<GemCtr>> matches = new()
     {
    { "vertical", new List<GemCtr>() },
    { "horizontal", new List<GemCtr>() }
     };

        foreach (var group in directionGroups)
        {
            foreach (var dir in group.Value)
            {
                checkDirection(gem, dir, matches[group.Key]);
            }
        }

        if (IsValidForMatch(gem))
        {
            matches["vertical"].Insert(matches["vertical"].Count / 2, gem);
            matches["horizontal"].Insert(matches["horizontal"].Count / 2, gem);
        }
        var verticalGems = matches["vertical"];
        var horizontalGems = matches["horizontal"];

       // Debug.Log($"After insert center: Vertical = {verticalGems.Count}, Horizontal = {horizontalGems.Count}");

        if (verticalGems.Count >= 3 && horizontalGems.Count >= 3)
        {
            Debug.Log("an chu V gem " + gem.GemType);
            List<GemCtr> connectedgems = new();

            connectedgems.AddRange(verticalGems);
            foreach (var h in horizontalGems)
            {
                if (!connectedgems.Contains(h))
                    connectedgems.Add(h);
            }

            return new MatchResult
            {
                connectedgems = connectedgems,
                direction = MatchDirection.super
            };
        }

        if (horizontalGems.Count == 3 && verticalGems.Count < 3)
        {
            Debug.Log("an 3 gem ngang " + gem.GemType);
            return new MatchResult
            {
                connectedgems = horizontalGems,
                direction = MatchDirection.ngang
            };
        }
        else if (horizontalGems.Count > 3 && verticalGems.Count < 3)
        {
            Debug.Log("an nhiều hơn 3 gem ngang " + gem.GemType);
            return new MatchResult
            {
                connectedgems = horizontalGems,
                direction = MatchDirection.ngangdai
            };
        }

        if (verticalGems.Count == 3 && horizontalGems.Count < 3)
        {
            Debug.Log("an 3 gem dọc " + gem.GemType);
            return new MatchResult
            {
                connectedgems = verticalGems,
                direction = MatchDirection.doc
            };
        }
        else if (verticalGems.Count > 3 && horizontalGems.Count < 3)
        {
            Debug.Log("an nhiều hơn 3 gem dọc " + gem.GemType);
            return new MatchResult
            {
                connectedgems = verticalGems,
                direction = MatchDirection.docdai
            };
        }

        return new MatchResult
        {
            connectedgems = new List<GemCtr> { gem },
            direction = MatchDirection.none
        };
    }


    void checkDirection(GemCtr gem, Vector2Int direction, List<GemCtr> connectedgems)
    {
    GemType gemType = gem.GemType;
    int x = gem.xIndex + direction.x;
    int y = gem.yIndex + direction.y;

    while (x >= 0 && x < gemboardCtr.Gemboard.width && y >= 0 && y < gemboardCtr.Gemboard.height)
    {
        var node = gemboardCtr.Gemboard.gemBoardNode[x, y];
            if (!node.isUsable || node.Gem == null)
                break;
            GemCtr gemAround = node.Gem.GetComponent<GemCtr>();
            if (!IsValidForMatch(gemAround))
                break;
            if (gemAround != null && gemAround.GemType == gemType)
            {
                if (!connectedgems.Contains(gemAround))
                {
                    connectedgems.Add(gemAround);
                }

                x += direction.x;
                y += direction.y;
            }
            else
            {
                break;
            }
        }
        
    }
    private bool IsValidForMatch(GemCtr gem)
    {
        if (gem == null) return false;

        Node node = gemboardCtr.Gemboard.gemBoardNode[gem.xIndex, gem.yIndex];

        if (node.Tile == null) return true;
        return node.Tile.tileType == TileType.LockTile;
    }

    public class MatchResult
    {
        public List<GemCtr> connectedgems;
        public MatchDirection direction;
    }

}
public enum MatchDirection
{
    doc,
    ngang,
    docdai,
    ngangdai,
    super,
    chuV,
    none,
}
