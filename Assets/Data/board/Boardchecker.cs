using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
public class Boardchecker : NghiaMono
{
    protected static Boardchecker instance;
    public static Boardchecker Instance => instance;
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] protected GemSpawnCtr gemSpawnCtr;
    public static List<Vector2Int> bombsToSpawn = new();
    private bool isProcessingMatch = false;
    //private List<BombCtr> bombsToExplode = new();
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
                                   // if (g is BombCtr bomb)
                                       // bombsToExplode.Add(bomb);
                                }
                                
                            }
                           /* if (superMatched.direction == MatchDirection.super)
                            {
                                Vector2Int swapPos = gemboardCtr.LastSwapPos;

                                GemCtr centerGem = superMatched.connectedgems.OrderBy(g => Vector2Int.Distance(new Vector2Int(g.xIndex, g.yIndex),swapPos)).First();

                                bombsToSpawn.Add(new Vector2Int(centerGem.xIndex, centerGem.yIndex));
                            }*/
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
        isProcessingMatch = false;
    }
    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves)
    {
       
        isProcessingMatch = true;
        gemboardCtr.GemSwaper.isProccessingMove = true;
        foreach (GemCtr gem in gemSpawnCtr.GemSpawner.GemtoRemove)
        {
            gem.ItMatched = false;
            gemboardCtr.GameManagerCtr.ScoreManager.StreakValue ++;
        }   
      
       yield return StartCoroutine(gemSpawnCtr.GemSpawner.RemoveAndRefillGem(gemSpawnCtr.GemSpawner.GemtoRemove));
        while (!GemSpawner.Instance.IsSpawnDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        if (!gemboardCtr.Gemboard.isInitializingBoard && !gemboardCtr.Gemboard.isShuffling)
         {
          if (DeadLockChecker.Instance.IsDeadLock())
           {
                Debug.LogWarning("Is Dead Lock");
                yield return StartCoroutine(gemboardCtr.Gemboard.ShuffleBoard());
                ResetState();
                yield break;
            }  

        if  (checkBoard())
            {
                yield return StartCoroutine(ProcessTurnOnMatchedBoard(false));
                
            }
           }
        gemboardCtr.GameManagerCtr.ScoreManager.IncreaseScore(gemboardCtr.GameManagerCtr.ScoreManager.basePieceValue * gemboardCtr.GameManagerCtr.ScoreManager.StreakValue);
        ResetState();
    }
      

    protected MatchResult SuperMached(MatchResult _MachedResult)
    {
        List<GemCtr> combinedGems = new(_MachedResult.connectedgems);
        bool foundSuperMatch = false;

        if (_MachedResult.direction == MatchDirection.chuV)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> extraGems = new();
                checkDirection(gem, new Vector2Int(1, 0), extraGems);
                checkDirection(gem, new Vector2Int(-1, 0), extraGems);
                checkDirection(gem, new Vector2Int(0, 1), extraGems);
                checkDirection(gem, new Vector2Int(0, -1), extraGems);

                if (extraGems.Count >= 3)
                {
                    Debug.LogWarning("an 5 gem " + gem.GemType);
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
        else if (_MachedResult.direction == MatchDirection.doc || _MachedResult.direction == MatchDirection.docdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> extraGems = new();
                checkDirection(gem, new Vector2Int(0, 1), extraGems);
                checkDirection(gem, new Vector2Int(0, -1), extraGems);

                if (extraGems.Count >= 5)
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

                if (extraGems.Count >= 5)
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

        // Add the center gem manually
        matches["vertical"].Insert(matches["vertical"].Count / 2, gem);
        matches["horizontal"].Insert(matches["horizontal"].Count / 2, gem);

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
                direction = MatchDirection.chuV
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
        if (node.isUsable && node.Gem != null)
        {
            GemCtr gemAround = node.Gem.GetComponent<GemCtr>();
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
        else
        {
            break;
        }
    }
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
