using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class Gemboard : NghiaMono
{  
    
    protected static Gemboard instance;
    public static Gemboard Instance { get => instance; }
    protected override void Awake()
    {
        base.Awake();
        if (Gemboard.instance != null) Debug.LogError("only one Gemboard allow to exist");
        Gemboard.instance = this;
       
    }
    
    [SerializeField] GemSpawnCtr gemSpawnCtr;
    [SerializeField] GemBoardCtr gemBoardCtr;

    public List<GameObject> GemtoDestroy = new();
    public List<GameObject> BomtoDestroy = new();
    
    public int width = 6;
    public int height = 8;
    
    public float spacingX;
    public float spacingY;

    public Node[,] gemBoardNode;
    
    public GameObject GemBoardGO;

    public ArrayLayout arrayLayout;

    

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemBoardCtrl();
        this.LoadGemSpawnCtrl();
     
    }

    protected override void Start()
    {
        base.Start();
        this.InitializaBoard();
    }
    protected virtual void LoadGemSpawnCtrl()
    {
        if (this.gemSpawnCtr != null) return;
        this.gemSpawnCtr = Transform.FindAnyObjectByType<GemSpawnCtr>();
        Debug.Log(transform.name + " :LoadGemCtrl", gameObject);
    }
   
    protected virtual void LoadGemBoardCtrl()
    {
        if (this.gemBoardCtr != null) return;
        this.gemBoardCtr = GetComponent<GemBoardCtr>();
        Debug.Log(transform.name + " :LoadGemBoardCtrl", gameObject);
    }
    public void InitializaBoard()
    {
        this.DestroyGem();
        gemBoardNode = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = ((float)(height - 1) / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);
                if (arrayLayout.rows[y].row[x])
                {
                    gemBoardNode[x, y] = new Node(false, null);
                 
                }
                else
                {
                    
                    Transform prefab = this.gemSpawnCtr.GemSpawner.RandomPrefab();
                  
                    Transform Gem = this.gemSpawnCtr.GemSpawner.Spawn(prefab, pos, Quaternion.identity);
                    Gem.GetComponent<GemCtr>().SetIndicies(x, y);
                    gemBoardNode[x, y] = new Node(true, Gem.gameObject);
                    Gem.gameObject.SetActive(true);
                    GemtoDestroy.Add(Gem.gameObject);
                    BomtoDestroy.Add(Gem.gameObject);
                }

            }
        }

        if (this.gemBoardCtr.Boardchecker.checkBoard())
        {
            Debug.Log("co cap giong nhau");
            this.InitializaBoard();
        }
        else
        {
            Debug.Log("khong con cap giong nhau");
          
        }
    }
    protected virtual void DestroyGem()
    {
         
        if (GemtoDestroy != null )
        {
            foreach (GameObject gem in GemtoDestroy)
            {
                Destroy(gem);
                this.gemSpawnCtr.GemSpawner.Spawncountdown();          
            }         
            foreach (GameObject bom in BomtoDestroy)
            {
                Destroy(bom);
            }
            GemtoDestroy.Clear();
            BomtoDestroy.Clear();
        }
    }
   
   
}



