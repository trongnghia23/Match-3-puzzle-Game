using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

public class GemSpawner : spawner
{
    protected static GemSpawner instance;
    public static GemSpawner Instance { get => instance; }
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] public List<GemCtr> GemtoRemove;
public bool IsSpawnDone => spawnTime == 0;

private int spawnTime = 0;

public void SpawnTimeUp()
{
    spawnTime++;
}

public void SpawnTimeDown()
{
    spawnTime = Mathf.Max(0, spawnTime - 1);
}
    protected override void Awake()
    {
        base.Awake();
        if (GemSpawner.instance != null) Debug.LogError("only one GemSpawner allow to exist");
        GemSpawner.instance  = this;
    }
protected override void Loadcomponents()
{
    base.Loadcomponents();
    this.LoadGemboardCtr();
}
    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = Transform.FindAnyObjectByType<GemBoardCtr>();
         Debug.Log(transform.name + " :LoadGemboardCtr", gameObject);
    }
   public virtual IEnumerator RemoveAndRefillGem(List<GemCtr> GemtoRemove)
    {
        
        foreach (GemCtr gem in GemtoRemove)
        {   
            int _xIndex = gem.xIndex;
            int _yIndex = gem.yIndex;
            gem.GemDespawn.DespawnObject();
            this.Spawncountdown();
            gemboardCtr.Gemboard.gemBoardNode[_xIndex,_yIndex] = new Node(true,null);
           
         
        }
         yield return new WaitForSeconds(0.05f);
        for (int x = 0; x < gemboardCtr.Gemboard.width; x++)
        {
            for (int y = 0; y < gemboardCtr.Gemboard.height; y++)
            {
                if (gemboardCtr.Gemboard.gemBoardNode[x,y].Gem == null)
                {
                   

                  //  Debug.Log("vi tri X:"+ x + "Y:" + y +"dang trong");
                    RefillGem(x,y);
                }
            }
        }
    }
   
    protected virtual void RefillGem(int x, int y)
    {  
        int yOffSet = 1;
        
        while (y + yOffSet < gemboardCtr.Gemboard.height && gemboardCtr.Gemboard.gemBoardNode[x,y + yOffSet].Gem == null)
        {
            yOffSet++;
        }   
           
        if (y + yOffSet < gemboardCtr.Gemboard.height && gemboardCtr.Gemboard.gemBoardNode[x,y + yOffSet].Gem != null)
        {
            if (gemboardCtr.Gemboard.arrayLayout.rows[y].row[x])
                {
                    gemboardCtr.Gemboard.gemBoardNode[x, y] = new Node(false, null);
                 
                }
                else
                {
                    GameObject gemObject = gemboardCtr.Gemboard.gemBoardNode[x,y + yOffSet].Gem;
                    GemCtr gemCtr = gemObject.GetComponent<GemCtr>();
                    Vector3 TargetPos = new Vector3(x - gemboardCtr.Gemboard.spacingX, y - gemboardCtr.Gemboard.spacingY, gemObject.transform.position.z);
                    
            gemCtr.GemMove.MoveToTarget(TargetPos);
            gemCtr.SetIndicies(x, y);

            gemboardCtr.Gemboard.gemBoardNode[x, y] = gemboardCtr.Gemboard.gemBoardNode[x, y + yOffSet];
            gemboardCtr.Gemboard.gemBoardNode[x, y + yOffSet] = new Node(true, null);
          
                }
        } 
        if (y + yOffSet == gemboardCtr.Gemboard.height)
        {
            SpawnGemOnTop(x, y);
        }
    }

    protected virtual void SpawnGemOnTop(int x, int y)
    {     
        int Index = FindIndexOfLowestGem(x);
        
        int LocationToMoveTo = gemboardCtr.Gemboard.height - Index;

        if (gemboardCtr.Gemboard.arrayLayout.rows[y].row[x])
        {
            gemboardCtr.Gemboard.gemBoardNode[x, y] = new Node(false, null);
        }
        else
        {
            Transform NewPrefab = this.RandomPrefab();
            Transform NewGem = this.Spawn(NewPrefab, new Vector2(x - gemboardCtr.Gemboard.spacingX, gemboardCtr.Gemboard.height - gemboardCtr.Gemboard.spacingY), Quaternion.identity);
            NewGem.gameObject.SetActive(true);
            this.Spawncountup();
            NewGem.GetComponent<GemCtr>().SetIndicies(x, Index);
            gemboardCtr.Gemboard.gemBoardNode[x, Index] = new Node(true, NewGem.gameObject);
            Vector3 targetPos = new Vector3(NewGem.transform.position.x, NewGem.transform.position.y - LocationToMoveTo, NewGem.transform.position.z);
            NewGem.GetComponent<GemCtr>().GemMove.MoveToTarget(targetPos);
        }
      
        
    }

    protected virtual int FindIndexOfLowestGem(int x)
    {
        int LowestNull = 99; 
        for (int y = gemboardCtr.Gemboard.height - 1; y >= 0; y--)
        {
            if (gemboardCtr.Gemboard.gemBoardNode[x,y].Gem == null)
            {  
                if (gemboardCtr.Gemboard.arrayLayout.rows[y].row[x])
                {
                    gemboardCtr.Gemboard.gemBoardNode[x, y] = new Node(false, null);
                }
                else
                {
                    LowestNull = y;
                }
            }
        }
        return LowestNull;
    }
}
