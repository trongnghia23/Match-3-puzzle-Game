using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
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
                if (gemboardCtr.Gemboard.gemBoardNode[x, y].isUsable && gemboardCtr.Gemboard.gemBoardNode[x, y].Gem != null)
                {
                    GemCtr gem = gemboardCtr.Gemboard.gemBoardNode[x, y].Gem.GetComponent<GemCtr>();
                    if (gem != null && !gem.ItMatched)
                    {
                        MatchResult MachedGem = IsConnected(gem);

                        if (MachedGem.connectedgems.Count >= 3)
                        {
                            MatchResult superMachedGem = SuperMached(MachedGem);

                            gemSpawnCtr.GemSpawner.GemtoRemove.AddRange(superMachedGem.connectedgems);
                            foreach (GemCtr gems in superMachedGem.connectedgems)
                            
                                gems.ItMatched = true;
                            

                            hasMatch = true;
                        }
                    }
                }
            }
        }
        
        
        return hasMatch;
    }
    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves)
    {
       gemboardCtr.GemSwaper.isProccessingMove = true;
        foreach (GemCtr gem in gemSpawnCtr.GemSpawner.GemtoRemove)
        {
            gem.ItMatched = false;
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

            // Đánh dấu trạng thái đang xáo
            yield return StartCoroutine(gemboardCtr.Gemboard.ShuffleBoard());

            // Chờ xáo xong trước khi tiếp tục (có thể bạn cần sửa ShuffleBoard để yield return tương tự)
            yield break;  // Kết thúc coroutine, chờ lần sau shuffle xong sẽ tiếp tục
        }

        if (checkBoard())
        {
            

            // Đệ quy nhưng có yield return để đợi xử lý xong
            yield return StartCoroutine(ProcessTurnOnMatchedBoard(false));

            yield break;
        }
    }

    gemboardCtr.GemSwaper.isProccessingMove = false;
        
    }

    protected MatchResult SuperMached(MatchResult _MachedResult)
    {
        if (_MachedResult.direction == MatchDirection.chuV)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> EXconnectedgems = new();
                checkDirection(gem, new Vector2Int(1, 0), EXconnectedgems);
                checkDirection(gem, new Vector2Int(-1, 0), EXconnectedgems);
                checkDirection(gem, new Vector2Int(0, 1), EXconnectedgems);
                checkDirection(gem, new Vector2Int(0, -1), EXconnectedgems);
                if (EXconnectedgems.Count >= 3)
                {
                    Debug.LogWarning("an 5 gem" + gem.GemType);
                    EXconnectedgems.AddRange(_MachedResult.connectedgems);
                    return new MatchResult
                    {
                        connectedgems = EXconnectedgems,
                        direction = MatchDirection.super
                    };
                }
            }
            return new MatchResult()
            {
                connectedgems = _MachedResult.connectedgems,
                direction = _MachedResult.direction
            };
        }    
        else if (_MachedResult.direction == MatchDirection.doc || _MachedResult.direction == MatchDirection.docdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> EXconnectedgems = new();
                checkDirection(gem, new Vector2Int(0, 1), EXconnectedgems);
                checkDirection(gem, new Vector2Int(0, -1), EXconnectedgems);
                if (EXconnectedgems.Count >= 3)
                {
                    Debug.LogWarning("an ngang sieu cap" + gem.GemType);
                    EXconnectedgems.AddRange(_MachedResult.connectedgems);
                    return new MatchResult
                    {
                        connectedgems = EXconnectedgems,
                        direction = MatchDirection.super
                    };
                }
            }
            return new MatchResult()
            {
                connectedgems = _MachedResult.connectedgems,
                direction = _MachedResult.direction
            };
        }    
        else if (_MachedResult.direction == MatchDirection.ngang || _MachedResult.direction == MatchDirection.ngangdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> EXconnectedgems = new();
                checkDirection(gem, new Vector2Int(1, 0), EXconnectedgems);
                checkDirection(gem, new Vector2Int(-1, 0), EXconnectedgems);
                if (EXconnectedgems.Count >= 3)
                {
                    Debug.LogWarning("an doc sieu cap" + gem.GemType);
                    EXconnectedgems.AddRange(_MachedResult.connectedgems);
                    return new MatchResult
                    {
                        connectedgems = EXconnectedgems,
                        direction = MatchDirection.super
                    };
                }
            }
            return new MatchResult()
            {
                connectedgems = _MachedResult.connectedgems,
                direction = _MachedResult.direction
            };
        } 
        
        return null;      
    }
   
   MatchResult IsConnected(GemCtr gem)
{
    // Group direction vectors
    Dictionary<string, List<Vector2Int>> directionGroups = new()
    {
        { "vertical", new List<Vector2Int> { new(0, 1), new(0, -1) } },
        { "horizontal", new List<Vector2Int> { new(1, 0), new(-1, 0) } }
    };

    // Store vertical and horizontal matches
    Dictionary<string, List<GemCtr>> matches = new()
    {
        { "vertical", new List<GemCtr> { gem } },
        { "horizontal", new List<GemCtr> { gem } }
    };

    // Check directions once
    foreach (var group in directionGroups)
    {
        foreach (var dir in group.Value)
        {
            checkDirection(gem, dir, matches[group.Key]);
        }
    }

    var verticalGems = matches["vertical"];
    var horizontalGems = matches["horizontal"];

    // Priority 1: chuV (T or + shape)
    if (verticalGems.Count >= 3 && horizontalGems.Count >= 3)
    {
        Debug.Log("chu V gem " + gem.GemType);
        List<GemCtr> connectedgems = new();

        // Add vertical gems
        foreach (var v in verticalGems)
        {
            if (!connectedgems.Contains(v))
                connectedgems.Add(v);
        }

        // Add horizontal gems
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

    // Priority 2: horizontal line
    if (horizontalGems.Count == 3)
    {
        Debug.Log("3 gem ngang " + gem.GemType);
        return new MatchResult
        {
            connectedgems = horizontalGems,
            direction = MatchDirection.ngang
        };
    }
    else if (horizontalGems.Count > 3)
    {
        Debug.Log("nhiều hơn 3 gem ngang " + gem.GemType);
        return new MatchResult
        {
            connectedgems = horizontalGems,
            direction = MatchDirection.ngangdai
        };
    }

    // Priority 3: vertical line
    if (verticalGems.Count == 3)
    {
        Debug.Log("3 gem dọc " + gem.GemType);
        return new MatchResult
        {
            connectedgems = verticalGems,
            direction = MatchDirection.doc
        };
    }
    else if (verticalGems.Count > 3)
    {
        Debug.Log("nhiều hơn 3 gem dọc " + gem.GemType);
        return new MatchResult
        {
            connectedgems = verticalGems,
            direction = MatchDirection.docdai
        };
    }

    // No match
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
            if (gemAround != null && !gemAround.ItMatched && gemAround.GemType == gemType)
            {
                if (!connectedgems.Contains(gemAround)) // ✅ prevent duplicates
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
