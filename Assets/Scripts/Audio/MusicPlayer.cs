using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioData song;
    public bool playOnAwake = true;
    void Start()
    {
        if (playOnAwake) PlaySong();
    }
    public void PlaySong()
    {
        MusicManager.Instance.PlaySong(song);
    }
}
