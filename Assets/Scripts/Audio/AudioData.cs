using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public enum Type
    {
        SFX,
        MUSIC
    }

    public AudioClip[] audioClips;
    public bool useRandomClip = false;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField, Range(0, 0.2f)] private float volumeRandom = 0;
    [SerializeField, Range(-24, 24)] private float pitch = 0;
    [SerializeField, Range(0, 12)] private float pitchRandom = 0;
    [SerializeField, Range(0, 1)] private float spacialBlend = 1;
    [SerializeField] private bool loop = false;
    [SerializeField] private Type type = Type.SFX;
    [SerializeField, Range(0f, 1f)] private float chance = 1;

    [Header("Pitch Lerp Settings")]
    [SerializeField] private bool usePitchLerp = false;
    [SerializeField, Range(0.1f, 10f)] private float pitchLerpDuration = 1f;

    [Header("Start Delay Settings")]
    [SerializeField, Range(0f, 10f)] private float startDelay = 0f;

    private int currentIndex = -1;

    public AudioSourceController Play(Transform parent, int id = 0)
    {
        AudioSourceController controller = Play(parent.position, id);
        controller.SetParent(parent);

        return controller;
    }

    public AudioSourceController Play(Vector3 position, int id = 0)
    {
        float rand = Random.Range(0, 100) * 0.01f;
        AudioSourceController controller = AudioManager.Instance.GetController(type);
        float volume = this.volume + Random.Range(-volumeRandom, volumeRandom);
        float pitch = AudioUtilities.SemitoneToPitch(this.pitch + Random.Range(-pitchRandom, pitchRandom));
        if (!useRandomClip && audioClips.Length > 0 && id != audioClips.Length - 1) controller.SetSourceProperties(GetAudioClip(id), volume, pitch, false, spacialBlend, usePitchLerp, pitchLerpDuration, startDelay);
        else controller.SetSourceProperties(GetAudioClip(id), volume, pitch, loop, spacialBlend, usePitchLerp, pitchLerpDuration, startDelay);
        controller.SetPosition(position);

        if (rand <= chance) controller.Play();

        return controller;
    }

    private AudioClip GetAudioClip(int id = 0)
    {
        if (audioClips.Length == 0) return null;
        int index = 0;
        if (useRandomClip) index = (audioClips.Length == 1) ? 0 : Random.Range(0, audioClips.Length);
        else index = id;
        if (index >= audioClips.Length) index = audioClips.Length - 1;
        currentIndex = index;
        //Debug.Log(id + " " + audioClips.Length);
        return audioClips[index];
    }

    public bool GetLoop()
    {
        return loop;
    }
}
