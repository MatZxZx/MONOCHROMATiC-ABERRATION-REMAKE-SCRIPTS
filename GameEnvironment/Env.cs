using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Env : MonoBehaviour
{
    public static Env Instance;
    public AudioSource musicSrc, sfxSrc;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        Application.targetFrameRate = 60;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("screenshot.png", 2);
        }

    }

    private void Play(AudioSource audioSrc, AudioClip sfx, float volume, float pitch)
    {
        audioSrc.clip = sfx;
        audioSrc.volume = volume;
        audioSrc.pitch = pitch;
        audioSrc.PlayOneShot(sfx);    
    }

    private IEnumerator PlaySound(AudioSource audioSrc, AudioClip sfx, float volume, float pitch = 1, float waitingTime = 0, bool loop = false)
    {
        yield return new WaitForSeconds(waitingTime);
        Play(audioSrc, sfx, volume, pitch);
        yield return "Audio Played";
        audioSrc.loop = loop;
    }

    private IEnumerator ChangeMusic(AudioSource audioSrc, AudioClip music, float volume, float waitingTime)
    {

        float lastAudioTime = audioSrc.time;
        AudioClip lastAudio = audioSrc.clip;

        audioSrc.clip = music;
        audioSrc.volume = volume;
        audioSrc.loop = false;
        audioSrc.Play();

        yield return new WaitForSeconds(waitingTime);

        audioSrc.clip = lastAudio;
        audioSrc.volume = volume;
        //audioSrc.SetScheduledStartTime(lastAudioTime);
        audioSrc.loop = true;
        audioSrc.SetScheduledStartTime(lastAudioTime);
        audioSrc.PlayScheduled(AudioSettings.dspTime);

        yield return "Audio Played";
    }

}
