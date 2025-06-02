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
        foreach (GemCtr gem in gemSpawnCtr.GemSpawner.GemtoRemove)
        {
            gem.ItMatched = false;
        }   

        gemSpawnCtr.GemSpawner.RemoveAndRefillGem(gemSpawnCtr.GemSpawner.GemtoRemove);
        yield return new WaitForSeconds(0.4f);

        if (checkBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
    }

    protected MatchResult SuperMached(MatchResult _MachedResult)
    {
        if (_MachedResult.direction == MatchDirection.ngang || _MachedResult.direction == MatchDirection.ngangdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> EXconnectedgems = new();
                checkDirection(gem, new Vector2Int(1, 0), EXconnectedgems);
                checkDirection(gem, new Vector2Int(-1, 0), EXconnectedgems);
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
        else if (_MachedResult.direction == MatchDirection.doc || _MachedResult.direction == MatchDirection.docdai)
        {
            foreach (GemCtr gem in _MachedResult.connectedgems)
            {
                List<GemCtr> EXconnectedgems = new();
                checkDirection(gem, new Vector2Int(0, 1), EXconnectedgems);
                checkDirection(gem, new Vector2Int(0, -1), EXconnectedgems);
                if (EXconnectedgems.Count >= 3)
                {
                    Debug.LogWarning("an doc sieu cap" + gem.GemType);
                    EXconnectedgems.AddRange(_MachedResult.connectedgems);
                    
                    // Spawn bomb at the center gem's position
                   
////////////////////////////
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
        List<GemCtr> connectedgems = new();
        GemType gemType = gem.GemType;
        connectedgems.Add(gem);

            //check phai
            checkDirection(gem, new Vector2Int(1, 0), connectedgems);
            //check trai
            checkDirection(gem, new Vector2Int(-1, 0), connectedgems);
            //an 3 gem
            if (connectedgems.Count == 3)
            {
                Debug.Log("3 gem ngang" + connectedgems[0].GemType);
                return new MatchResult
                {
                    connectedgems = connectedgems,
                    direction = MatchDirection.ngang
                };
            }
            //an nhieu hon 3 gem
            else if (connectedgems.Count > 3)
            {
                Debug.Log("nhiều hơn 3 gem" + connectedgems[0].GemType);
                return new MatchResult
                {
                    connectedgems = connectedgems,
                    direction = MatchDirection.ngangdai
                };
            }
            // xoa ket qua
            connectedgems.Clear();
            // doc loai ngoc dau tien
            connectedgems.Add(gem);
            //check tren
            checkDirection(gem, new Vector2Int(0, 1), connectedgems);
            //check duoi
            checkDirection(gem, new Vector2Int(0, -1), connectedgems);
            //an 3 gem
            if (connectedgems.Count == 3)
            {
                Debug.Log("3 gem doc" + connectedgems[0].GemType);
                return new MatchResult
                {
                    connectedgems = connectedgems,
                    direction = MatchDirection.doc
                };
            }
            //an nhieu hon 3 gem
            else if (connectedgems.Count > 3)
            {
                Debug.Log("nhiều hơn 3 gem" + connectedgems[0].GemType);
                return new MatchResult
                {
                    connectedgems = connectedgems,
                    direction = MatchDirection.docdai
                };
            }else
            {
                return new MatchResult
                {
                    connectedgems = connectedgems,
                    direction = MatchDirection.none
                };
            }
        }
    void checkDirection(GemCtr gem, Vector2Int direction, List<GemCtr> connectedgems)
    {
        GemType gemType = gem.GemType;
        int x = gem.xIndex + direction.x;
        int y = gem.yIndex + direction.y;

        while (x >= 0 && x < gemboardCtr.Gemboard.width && y >= 0 && y < gemboardCtr.Gemboard.height)
        {
            if (gemboardCtr.Gemboard.gemBoardNode[x, y].isUsable && gemboardCtr.Gemboard.gemBoardNode[x, y].Gem != null)
            {
                GemCtr gemAround = gemboardCtr.Gemboard.gemBoardNode[x, y].Gem.GetComponent<GemCtr>();
                if (gemAround != null && !gemAround.ItMatched && gemAround.GemType == gemType)
                {
                    connectedgems.Add(gemAround);
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
    none,
}
