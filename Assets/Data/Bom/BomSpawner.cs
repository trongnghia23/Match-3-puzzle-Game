using UnityEngine;
using System.Collections.Generic;

public class BomSpawner : spawner
{
    protected static BomSpawner instance;
    public static BomSpawner Instance => instance;

    [SerializeField] protected GemBoardCtr gemboardCtr;
    [SerializeField] public List<GemCtr> GemtoRemove;

    protected override void Awake()
    {
        base.Awake();
        if (BomSpawner.instance != null)
        {
            Debug.LogError("Only one BomSpawner allowed to exist");
            return;
        }
        BomSpawner.instance = this;
    }
    private void Start()
    {
        if (gemboardCtr == null)
        {
            Debug.LogError("gemboardCtr is null in BomSpawner");
        }
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
        this.LoadGemboardCtr();
    }

    protected virtual void LoadGemboardCtr()
    {
        if (this.gemboardCtr != null) return;
        this.gemboardCtr = FindAnyObjectByType<GemBoardCtr>();
        Debug.Log(transform.name + ": LoadGemboardCtr", gameObject);
    }

    /// <summary>
    /// Spawn bomb tại vị trí (x,y) trên board
    /// </summary>
    public virtual void SpawnBombAt(int x, int y)
    {
        Node[,] board = gemboardCtr.Gemboard.gemBoardNode;
        Node node = board[x, y];

        if (node.Gem != null)
        {
            GemCtr oldGem = node.Gem.GetComponent<GemCtr>();
            if (oldGem != null && !oldGem.ItMatched)
            {
                oldGem.ItMatched = true;
                GemSpawner.Instance.GemtoRemove.Add(oldGem); // ← Để RemoveAndRefillGem xử lý
            }
            node.Gem = null; // Gỡ liên kết trong node, không Destroy ở đây!
        }
        
        Vector3 spawnPos = gemboardCtr.Gemboard.GetWorldPosFromXY(x, y);
        Transform bombTf = this.Spawn("BoxBom", spawnPos, Quaternion.identity);
        if (bombTf == null)
        {
            Debug.LogError($"Failed to spawn bomb at ({x},{y})");
            return;
        }

        bombTf.gameObject.SetActive(true);

        BombCtr bombCtr = bombTf.GetComponent<BombCtr>();
        bombCtr.SetIndicies(x, y);

        node.Gem = bombTf.gameObject;
     
    }
}
