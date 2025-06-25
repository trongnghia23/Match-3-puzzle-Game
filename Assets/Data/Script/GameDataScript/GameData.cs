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
    public static GameData Gamedata;
   public Savedata savedata;

    protected override void Awake()
    {
      
        if (Gamedata == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Gamedata = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();
    }
    protected override void Start()
    {
        base.Start();
        
    }
    public virtual void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/Player.dat", FileMode.Create);
        Savedata data = new Savedata();
        data = savedata;
        formatter.Serialize(file, data);
        file.Close();
        Debug.Log("Save");
    }
    public virtual void Load() 
    {
        if (File.Exists(Application.persistentDataPath + "/Player.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Player.dat", FileMode.Open);
            savedata = formatter.Deserialize(file) as Savedata;
            file.Close();
            Debug.Log("Load");
        }
        else
        {
            savedata = new Savedata();
            savedata.IsActive = new bool[100];
            savedata.Stars = new int[100];
            savedata.HighScores = new int[100];
            savedata.IsActive[0] = true;
        }
    }
    private void OnDisable()
    {
        Save();
    }
}
