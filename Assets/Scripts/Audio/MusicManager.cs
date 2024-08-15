using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioData currentSong;
    AudioSourceController currentSongController;
    // Start is called before the first frame update
    private void Start()
    {
        if (currentSong != null)
        {
            currentSongController = currentSong.Play(transform);
        }
    }
    public void PlaySong(AudioData song)
    {
        //Debug.Log(song.name);
        if (currentSongController != null) currentSongController.Stop();
        currentSong = song;
        currentSongController = currentSong.Play(transform);
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
