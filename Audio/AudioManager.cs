using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource src;
    private void Awake()
    {
        if (src == null)
        {
            src = GetComponent<AudioSource>();
        }
    }

    public static void PlayOneShot(string sound, float volume = 1, float pitch = 1)
    {
        AudioClip clip = Resources.Load<AudioClip>(sound);
        src.clip = clip;
        src.pitch = pitch;
        src.PlayOneShot(clip, volume);
    }
    public static void Play(AudioClip sound, float volume = 1, float pitch = 1)
    {
        src.clip = sound;
        src.volume = volume;
        src.pitch = pitch;
        src.Play();
    }
    

}
