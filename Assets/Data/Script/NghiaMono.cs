using UnityEngine;

public class NghiaMono : MonoBehaviour
{
    protected virtual void Awake()
    {

        this.Loadcomponents();
    }
    protected virtual void Start()
    {
        // For overr
    }
    protected virtual void Reset()
    {
        this.Loadcomponents();
        this.ResetValue();
    }
    protected virtual void Loadcomponents()
    {
        // For overr
    }
    protected virtual void ResetValue()
    {
        // For overr
    }
    protected virtual void OnEnable()
    {
        // For overr
    }
}
