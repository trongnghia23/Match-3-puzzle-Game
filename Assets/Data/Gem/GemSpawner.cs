using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

public class GemSpawner : spawner
{
    protected static GemSpawner instance;
    public static GemSpawner Instance { get => instance; }
    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] public List<GemCtr> GemtoRemove ;
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
   public virtual void RemoveAndRefillGem(List<GemCtr> GemtoRemove)
    {
     
        foreach (GemCtr gem in GemtoRemove)
        {   
            int _xIndex = gem.xIndex;
            int _yIndex = gem.yIndex;
            gem.GemDespawn.DespawnObject();
            this.Spawncountdown();
            gemboardCtr.Gemboard.gemBoardNode[_xIndex,_yIndex] = new Node(true,null);
         
        }
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
          //  Debug.LogWarning("hang tren dang trong,tang yOffSet len roi spawn. offset hien tai:" + yOffSet + "tang len 1");
            yOffSet++;
        }   
           
        if (y + yOffSet < gemboardCtr.Gemboard.height && gemboardCtr.Gemboard.gemBoardNode[x,y + yOffSet].Gem != null)
        {
            GemCtr gemAbove = gemboardCtr.Gemboard.gemBoardNode[x,y + yOffSet].Gem.GetComponent<GemCtr>();
            Vector3 TargetPos = new Vector3(x - gemboardCtr.Gemboard.spacingX, y - gemboardCtr.Gemboard.spacingY, gemAbove.transform.position.z);
           // Debug.Log("gem ben tren dang o vi tri:[" + x + "," + (y + yOffSet) + "] va se di chuyen den vi tri:[" + x + "," + y + "]");
            gemAbove.GemMove.MoveToTarget(TargetPos);
            gemAbove.SetIndicies(x, y);

            gemboardCtr.Gemboard.gemBoardNode[x, y] = gemboardCtr.Gemboard.gemBoardNode[x, y + yOffSet];
            gemboardCtr.Gemboard.gemBoardNode[x, y + yOffSet] = new Node(true, null);
        } 
        if (y + yOffSet == gemboardCtr.Gemboard.height)
        {
           // Debug.LogWarning("cham dinh cua bang");
            SpawnGemOnTop(x);
           
        }
    }

    protected virtual void SpawnGemOnTop(int x)
    {     
        int Index = FindIndexOfLowestGem(x);
        int LocationToMoveTo = 8 - Index;
       // Debug.Log("chuan bi spawn gem,spawn gem o vi tri:"+ Index );

        Transform NewPrefab = this.RandomPrefab();
        Transform NewGem = this.Spawn(NewPrefab, new Vector2(x - gemboardCtr.Gemboard.spacingX, gemboardCtr.Gemboard.height - gemboardCtr.Gemboard.spacingY), Quaternion.identity);
        NewGem.gameObject.SetActive(true);
       this.Spawncountup();

        NewGem.GetComponent<GemCtr>().SetIndicies(x, Index);
        gemboardCtr.Gemboard.gemBoardNode[x, Index] = new Node(true, NewGem.gameObject);
        Vector3 targetPos = new Vector3(NewGem.transform.position.x, NewGem.transform.position.y - LocationToMoveTo, NewGem.transform.position.z);
        NewGem.GetComponent<GemCtr>().GemMove.MoveToTarget(targetPos);
        
    }
    protected virtual int FindIndexOfLowestGem(int x)
    {
        int LowestNull = 99;
        for (int y = 7; y >= 0; y--)
        {
            if (gemboardCtr.Gemboard.gemBoardNode[x,y].Gem == null)
            {
                LowestNull = y;
            }
        }
        return LowestNull;
    } 

}
