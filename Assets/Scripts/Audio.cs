using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio
{
    //the code inside of this class is from https://www.youtube.com/watch?v=6OT43pvUyfY

    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool looping;

    [HideInInspector]
    public AudioSource source;
}
