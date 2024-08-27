using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioData song;
    public bool playOnAwake = false;
    public bool playOnStart = true;

    void Start()
    {
        if (playOnStart) PlaySong();
    }
    void Awake()
    {
        if (playOnAwake) PlaySong();
    }
    public void PlaySong()
    {
        Debug.Log(song.name);
        MusicManager.Instance.PlaySong(song);
    }
}
