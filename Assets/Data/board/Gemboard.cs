using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class Gemboard : NghiaMono
{  
    protected static Gemboard instance;
    public static Gemboard Instance { get => instance; }
 
    [SerializeField] GemSpawnCtr gemSpawnCtr;
    [SerializeField] GemBoardCtr gemBoardCtr;
    [SerializeField] protected Transform holder;
    [Header("BoardSize")]
    public int width;
    public int height;
    public float spacingX;
    public float spacingY;
    public GameObject BlankSpaces;
    public List<GameObject> GemtoDestroy = new();
    [Header("Layout")]
    public Node[,] gemBoardNode;
    public ArrayLayout arrayLayout;

    public bool isInitializingBoard = false;
    public bool isShuffling = false;
    protected override void Awake()
    {
        base.Awake();
        if (Gemboard.instance != null) Debug.LogError("only one Gemboard allow to exist");
        Gemboard.instance = this;
    }

    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemBoardCtrl();
        this.LoadGemSpawnCtrl();
        this.LoadHolder();
    }
    protected override void Start()
    {
        base.Start();
        this.InitializaBoard();
        gemBoardCtr.SetGameState(GemBoardCtr.GameState.Pause);

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
    protected virtual void LoadHolder()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.Log(transform.name + ": LoadHolder", gameObject);
    }
    public void InitializaBoard()
    {
        if (isInitializingBoard) return;
        isInitializingBoard = true;
        this.DestroyGem();
        gemBoardNode = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = ((float)(height - 1) / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 pos = new Vector2(x - spacingX, y - spacingY);
                if (arrayLayout != null && y < arrayLayout.rows.Length
                && x < arrayLayout.rows[y].row.Length && arrayLayout.rows[y].row[x])
                {
                    gemBoardNode[x, y] = new Node(false, null);
                 
                }
                else
                {
                    
                    Transform prefab = this.gemSpawnCtr.GemSpawner.RandomPrefab();
                    Transform Gem = this.gemSpawnCtr.GemSpawner.Spawn(prefab, pos, Quaternion.identity);
                    GameObject BlankSpace = Instantiate(BlankSpaces, pos, Quaternion.identity);
                    BlankSpace.transform.parent = this.holder;
                    Gem.GetComponent<GemCtr>().SetIndicies(x, y);
                    gemBoardNode[x, y] = new Node(true, Gem.gameObject);
                    Gem.gameObject.SetActive(true);
                    BlankSpace.gameObject.SetActive(true);
                    GemtoDestroy.Add(Gem.gameObject);
                    GemtoDestroy.Add(BlankSpace);
                }

            }
        }
        isInitializingBoard = false;
       bool hasMatchingPairs = this.gemBoardCtr.Boardchecker.checkBoard();
       bool isDeadLock = this.gemBoardCtr.DeadLockChecker.IsDeadLock();

if (hasMatchingPairs || isDeadLock)
{
    Debug.Log("Co cap giong nhau hoac deadlock - goi lai InitializaBoard");
            StartCoroutine(RetryInitializeBoard());
}
else
{
    Debug.Log("Khong con cap giong nhau va khong deadlock");
            FadePanelCtr fade = FindAnyObjectByType<FadePanelCtr>();
            fade.Loading();
            gemBoardCtr.Boardchecker.ResetState();
}
    }
    private IEnumerator RetryInitializeBoard()
    {
        yield return new WaitForSeconds(0.1f);
        InitializaBoard();
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
            GemtoDestroy.Clear();
        }
    }
    public Vector3 GetWorldPosFromXY(int x, int y)
    {
        float posX = x - spacingX;
        float posY = y - spacingY;
        return new Vector3(posX, posY, 0f);
    }
    public IEnumerator ShuffleBoard()
{
    if (isShuffling) yield break;
    isShuffling = true;

    int retry = 5;

    while (retry > 0)
    {
        gemBoardCtr.GemSwaper.isProccessingMove = true;
        List<GameObject> allGems = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (arrayLayout.rows[y].row[x]) continue;
                GameObject gem = gemBoardNode[x, y]?.Gem;
                if (gem != null) allGems.Add(gem);
            }
        }

        System.Random rng = new();
        int n = allGems.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (allGems[n], allGems[k]) = (allGems[k], allGems[n]);
        }

        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (arrayLayout.rows[y].row[x]) continue;
                GameObject gem = allGems[index];
                gemBoardNode[x, y].Gem = gem;

                Vector2 newPos = new(x - spacingX, y - spacingY);
                gem.GetComponent<GemCtr>().SetIndicies(x, y);
                gem.GetComponent<GemCtr>().GemMove.MoveToTarget(newPos);
                index++;
            }
        }

        yield return StartCoroutine(WaitUntilAllMoved());

        bool hasMatch = gemBoardCtr.Boardchecker.checkBoard();

        if (!hasMatch && !DeadLockChecker.Instance.IsDeadLock())
        {
            Debug.Log("Bảng xáo thành công, không match và không deadlock.");
            break; 
        }

        Debug.LogWarning("Sau khi xáo vẫn match hoặc deadlock, thử lại...");
        retry--;
    }

    if (DeadLockChecker.Instance.IsDeadLock())
    {
        Debug.LogError("Vẫn bị deadlock sau khi xáo nhiều lần!");
       
        yield return StartCoroutine(ShuffleBoard());
    }
    else
    {
        gemBoardCtr.GemSwaper.isProccessingMove = false;
    }
    isShuffling = false;
}

    public IEnumerator WaitUntilAllMoved()
    {
        while (!GemSpawner.Instance.IsSpawnDone)
        {
            yield return null;
        }
    }
}  




