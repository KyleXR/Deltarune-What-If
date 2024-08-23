using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(DestroyTimer))]
public class AudioSourceController : MonoBehaviour
{
    public AudioData.Type type;

    private AudioSource audioSource;
    private Transform parent;
    private bool active = false;
    private float playTime = 0;
    private DestroyTimer destroyTimer;
    private bool isQuitting = false;
    private Coroutine pitchLerpCoroutine;

    private float playDelay = 0f;
    private bool pitchLerp = false;
    private float pitchLerpDuration = 0f;

    public event Action OnClipFinished;
    private int clipIndex = 0;
    private float playStartTime = 0;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        destroyTimer = GetComponent<DestroyTimer>();
        destroyTimer.enabled = false;
        playStartTime = Time.time;
    }
    //private void Update()
    //{
    //    if (!audioSource.isPlaying && OnClipFinished != null)
    //    {
    //        OnClipFinished.Invoke();
    //    }
    //}
    void Update()
    {
        if (active && !audioSource.isPlaying)
        {
            Stop();
            if (OnClipFinished != null)
            {
                OnClipFinished.Invoke();
            }
        }

        if (parent != null)
        {
            transform.position = parent.position;
        }
    }

    public void SetSourceProperties(AudioClip clip, float volume, float pitch, bool loop, float spacialBlend, bool usePitchLerp, float pitchLerpDuration, float startDelay)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = usePitchLerp ? 0f : pitch; // Start pitch at 0 if using pitch lerp
        audioSource.loop = loop;
        audioSource.spatialBlend = spacialBlend;
        playDelay = startDelay;
        pitchLerp = usePitchLerp;
        this.pitchLerpDuration =  pitchLerpDuration;

        if (type == AudioData.Type.SFX && !loop)
        {
            destroyTimer.timeToDestroy = audioSource.clip.length;
            destroyTimer.enabled = true;
        }
    }

    public void Play()
    {
        if (playDelay > 0)
        {
            StartCoroutine(PlayWithDelay(playDelay));
        }
        else
        {
            PlayInternal();
        }
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayInternal(); // This will now only call PlayInternal after the delay
    }

    private void PlayInternal()
    {
        if (audioSource != null && !active)
        {
            active = true;
            audioSource.pitch = pitchLerp ? 0.01f : audioSource.pitch; // Set to small pitch if lerping

            if (pitchLerp)
            {
                if (pitchLerpCoroutine != null)
                {
                    StopCoroutine(pitchLerpCoroutine);
                }
                pitchLerpCoroutine = StartCoroutine(LerpPitch(audioSource.pitch, pitchLerpDuration));
            }
            audioSource.Play(); // Start playback
        }
    }


    public void SetParent(Transform parent)
    {
        this.parent = parent;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Stop()
    {
        if (audioSource != null) audioSource.Stop();
        Reset();
        if (!isQuitting) AudioManager.Instance.ReturnController(this);
    }

    private void Reset()
    {
        active = false;
        parent = null;
        if (pitchLerpCoroutine != null)
        {
            StopCoroutine(pitchLerpCoroutine);
            pitchLerpCoroutine = null;
        }
    }

    public void FadeVolume(float duration = 3)
    {
        StartCoroutine(FadeOut(duration));
    }

    private IEnumerator FadeOut(float duration)
    {
        if (audioSource != null)
        {
            float startVolume = audioSource.volume;
            float step = startVolume / duration;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= step * Time.deltaTime;
                yield return null;
            }

            audioSource.volume = 0f;
        }
    }

    private IEnumerator LerpPitch(float targetPitch, float duration)
    {
        Debug.Log("Starting pitch lerp...");
        float startPitch = 0.01f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, elapsedTime / duration);
            Debug.Log($"Pitch: {audioSource.pitch}");
            yield return null;
        }

        audioSource.pitch = targetPitch;
        Debug.Log("Pitch lerp complete.");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (audioSource != null && active) // Only resume if it was already playing
        {
            if (hasFocus)
            {
                audioSource.time = playTime;
                audioSource.Play();
            }
            else
            {
                playTime = audioSource.time;
                audioSource.Pause();
            }
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if (audioSource != null && active) // Only resume if it was already playing
        {
            if (isPaused)
            {
                playTime = audioSource.time;
                audioSource.Pause();
            }
            else
            {
                audioSource.time = playTime;
                audioSource.Play();
            }
        }
    }


    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}
