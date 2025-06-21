using UnityEngine;

public class FxSpawer : spawner
{
    protected static FxSpawer instance;
    public static FxSpawer Instance { get => instance; }
    public static string Smokeone = "Smoke_1";
    public static string Chose = "Chose";
    protected override void Awake()
    {
        base.Awake();
        if (FxSpawer.instance != null) Debug.LogError("only one FxSpawer allow to exist");  
        FxSpawer.instance = this;
    }
    protected override void Loadcomponents()
    {
        base.Loadcomponents();
    }
    
}
