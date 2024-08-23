using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioData currentSong;
    AudioSourceController currentSongController;
    [SerializeField] bool playOnAwake = true;
    // Start is called before the first frame update
    private void Start()
    {
        if (currentSong != null && playOnAwake)
        {
            currentSongController = currentSong.Play(transform);
        }
    }
    public void PlaySong(AudioData song)
    {
        //Debug.Log(song.name);
        if (currentSongController != null) currentSongController.Stop();
        //Destroy(currentSongController.gameObject);
        currentSong = song;
        currentSongController = currentSong.Play(transform);
        DontDestroyOnLoad(currentSongController);
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
