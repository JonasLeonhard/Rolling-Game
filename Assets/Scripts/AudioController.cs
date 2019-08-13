using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System;

public class AudioController : MonoBehaviour {

    //the code inside of this class is from https://www.youtube.com/watch?v=6OT43pvUyfY
    public Audio[] sounds;

    public static AudioController instance;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(Audio s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.looping;
        }
    }

    private void Start()
    {
        Play("Theme");
    }
    public void Play(string name)
    {
        Audio s = Array.Find(sounds, sound => sound.audioClip.name == name);
        Debug.Log("Play: " + s.source.clip.name);

        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.Log("Sound was not found " + gameObject.name);
        }
    }

    public Audio getAudio(string name)
    {
        return Array.Find(sounds, sound => sound.audioClip.name == name);
    }
}
