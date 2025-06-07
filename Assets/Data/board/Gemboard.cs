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
    public int width = 7;
    public int height = 9;
    public float spacingX;
    public float spacingY;
    public Node[,] gemBoardNode;
    public ArrayLayout arrayLayout;
    public bool isInitializingBoard = false;
    public bool isShuffling = false;
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
    public void InitializaBoard(int retry = 4)
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
                }

            }
        }
        isInitializingBoard = false;
       bool hasMatchingPairs = this.gemBoardCtr.Boardchecker.checkBoard();
       bool isDeadLock = DeadLockChecker.Instance.IsDeadLock();

if (retry > 0 && (hasMatchingPairs || isDeadLock))
{
    Debug.Log("Co cap giong nhau hoac deadlock - goi lai InitializaBoard");
    this.InitializaBoard(retry - 1);
}
else if (isDeadLock)
{
    Debug.Log("Deadlock - shuffle board");
    this.ShuffleBoard();
}
else
{
    Debug.Log("Khong con cap giong nhau va khong deadlock - khong goi lai InitializaBoard");
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
            GemtoDestroy.Clear();
        }
    }
public IEnumerator ShuffleBoard()
{
    if (isShuffling) yield break;
    isShuffling = true;

    int retry = 5;

    while (retry > 0)
    {
        gemBoardCtr.GemSwaper.isProccessingMove = true;
        // Bước 1: Gom tất cả gem hiện có
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

        // Bước 2: Fisher-Yates shuffle
        System.Random rng = new();
        int n = allGems.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (allGems[n], allGems[k]) = (allGems[k], allGems[n]);
        }

        // Bước 3: Gán lại gem đã shuffle vào vị trí
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

        // Đợi tất cả gem di chuyển xong
        yield return StartCoroutine(WaitUntilAllMoved());

        // Kiểm tra match sau khi xáo
        bool hasMatch = gemBoardCtr.Boardchecker.checkBoard();

        if (!hasMatch && !DeadLockChecker.Instance.IsDeadLock())
        {
            Debug.Log("Bảng xáo thành công, không match và không deadlock.");
            break; // thành công
        }

        Debug.LogWarning("Sau khi xáo vẫn match hoặc deadlock, thử lại...");
        retry--;
    }

    // Sau cùng, nếu vẫn còn deadlock thì đánh dấu
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




