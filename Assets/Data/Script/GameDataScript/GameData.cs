using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Savedata
{
    public bool[] IsActive;
    public int[] HighScores;
    public int[] Stars;
}
public class GameData : NghiaMono
{
    /* ---------- Singleton ---------- */
    public static GameData Instance { get; private set; }
    

    /* ---------- Data ---------- */
    public Savedata savedata;
    public event Action OnLoaded;

    /* ---------- Const ---------- */
    private const int TOTAL_LEVELS = 100;
    private string SavePath => Path.Combine(Application.persistentDataPath, "Player.json");

    /* ---------- Life-cycle ---------- */
    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Debug.Log("GameData Instance created", gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Load();

        // ✅ Đảm bảo mở rộng mảng NGAY trong Awake, trước khi các script khác dùng
        EnsureArraySize(ref savedata.IsActive, TOTAL_LEVELS);
        EnsureArraySize(ref savedata.Stars, TOTAL_LEVELS);
        EnsureArraySize(ref savedata.HighScores, TOTAL_LEVELS);
    }
    
    private void OnDisable()            // gọi khi thoát Play mode / app
    {
        Save();
    }

    /* ---------- Save / Load ---------- */
    public void Save()
    {
        EnsureArraySize(ref savedata.IsActive, TOTAL_LEVELS);
        EnsureArraySize(ref savedata.Stars, TOTAL_LEVELS);
        EnsureArraySize(ref savedata.HighScores, TOTAL_LEVELS);


        string json = JsonUtility.ToJson(savedata, true);
        File.WriteAllText(this.SavePath, json);
        Debug.Log($"[GameData] Saved to {this.SavePath}");
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                savedata = JsonUtility.FromJson<Savedata>(json);

                if (savedata == null)
                {
                    throw new Exception("Deserialized save data is null");
                }

                EnsureArraySize(ref savedata.IsActive, TOTAL_LEVELS);
                EnsureArraySize(ref savedata.Stars, TOTAL_LEVELS);
                EnsureArraySize(ref savedata.HighScores, TOTAL_LEVELS);

#if UNITY_EDITOR
                Debug.Log($"[GameData] Loaded from {SavePath}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GameData] Load failed: {e.Message}. Resetting save.");
                CreateNewSave();
                Save();
            }
        }
        else
        {
            CreateNewSave();
            Save();
        }
        EnsureArraySizePublic();
        OnLoaded?.Invoke();
    }



    /* ---------- Helpers ---------- */
    private static void EnsureArraySize<T>(ref T[] array, int size)
    {
        if (array == null || array.Length < size)
        {
            T[] newArr = new T[size];
            if (array != null) Array.Copy(array, newArr, array.Length);
            array = newArr;
        }
    }
    public void EnsureArraySizePublic()
    {
        EnsureArraySize(ref savedata.IsActive, 100);
        EnsureArraySize(ref savedata.Stars, 100);
        EnsureArraySize(ref savedata.HighScores, 100);
    }
    private void CreateNewSave()
    {
        savedata = new Savedata
        {
            IsActive = new bool[TOTAL_LEVELS],
            Stars = new int[TOTAL_LEVELS],
            HighScores = new int[TOTAL_LEVELS]
        };
        savedata.IsActive[0] = true; // mở level đầu tiên
    }
}
