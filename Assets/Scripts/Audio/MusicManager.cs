using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioData currentSong;
    AudioSourceController currentSongController;
    [SerializeField] bool playOnAwake = true;
    private int currentClipIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        if (currentSong != null && playOnAwake)
        {
            PlayCurrentClip();
        }
    }

    private void PlayCurrentClip()
    {
        if (currentSongController != null) currentSongController.Stop();
        //.Log(currentClipIndex);
        currentSongController = currentSong.Play(transform, currentClipIndex);
        currentSongController.OnClipFinished += HandleClipFinished;

        //DontDestroyOnLoad(currentSongController);
    }

    private void HandleClipFinished()
    {
        currentClipIndex++;
        //Debug.Log("Current clip: " + currentClipIndex + ", Total clips: " + currentSong.audioClips.Length);

        if (currentClipIndex < currentSong.audioClips.Length)
        {
            PlayCurrentClip();
            //Debug.Log("NextClip");
        }
        else if (currentSong.GetLoop())
        {
            currentClipIndex--;
            PlayCurrentClip();
        }
    }

    public void PlaySong(AudioData song)
    {
        if (currentSongController != null) currentSongController.Stop();

        currentSong = song;
        currentClipIndex = 0;
        PlayCurrentClip();
    }

    public void Stop()
    {
        if (currentSongController != null) currentSongController.Stop();
    }

    public void FadeSong(float duration = 3)
    {
        currentSongController.FadeVolume(duration);
    }
}
