using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Despawnnoise;
    public AudioSource[] BackgroundMusic;
    public AudioSource Movenoise;

    public virtual void PlayDespawNoise()
    {
        Despawnnoise.Play();
    }
    public virtual void PlayMoveNoise()
    {
        Movenoise.Play();
    }

}
