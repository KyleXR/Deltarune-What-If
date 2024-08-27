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
    private bool paused = false;
    private float playTime = 0;
    private DestroyTimer destroyTimer;
    private bool isQuitting = false;
    private Coroutine pitchLerpCoroutine;

    private float playDelay = 0f;
    private bool pitchLerp = false;
    private float pitchLerpDuration = 0f;

    public event Action OnClipFinished;
    //private int clipIndex = 0;
    private float playStartTime = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        destroyTimer = GetComponent<DestroyTimer>();
        destroyTimer.enabled = false;
        playStartTime = Time.time;
    }

    void Update()
    {
        if (active && !audioSource.isPlaying && !paused && !isQuitting)
        {
            Debug.Log("Stopped");
            Stop();
            OnClipFinished?.Invoke();
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
        audioSource.pitch = pitch; // Start pitch at 0.5 for audible sound
        audioSource.loop = loop;
        audioSource.spatialBlend = spacialBlend;
        playDelay = startDelay;
        pitchLerp = usePitchLerp;
        this.pitchLerpDuration = pitchLerpDuration;

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
        PlayInternal();
    }

    private void PlayInternal()
    {
        if (audioSource != null && !active && audioSource.gameObject.activeInHierarchy && !isQuitting)
        {
            active = true;
            //audioSource.pitch = pitchLerp ? 0.01f : audioSource.pitch; // Use an audible starting pitch

            audioSource.Play(); // Start playback
            if (pitchLerp)
            {
                if (pitchLerpCoroutine != null)
                {
                    StopCoroutine(pitchLerpCoroutine);
                }
                pitchLerpCoroutine = StartCoroutine(LerpPitch(audioSource.pitch, pitchLerpDuration));
            }
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
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        Reset();
        if (!isQuitting)
        {
            AudioManager.Instance.ReturnController(this);
        }
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
        float startPitch = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, elapsedTime / duration);
            yield return null;
        }
        Debug.Log(audioSource.pitch);
        audioSource.pitch = targetPitch;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (audioSource != null && active)
        {
            if (hasFocus)
            {
                audioSource.time = playTime;
                audioSource.Play();
                paused = false;
            }
            else
            {
                playTime = audioSource.time;
                audioSource.Pause();
                paused = true;
            }
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if (audioSource != null && active)
        {
            if (isPaused)
            {
                playTime = audioSource.time;
                audioSource.Pause();
                paused = true;
            }
            else
            {
                audioSource.time = playTime;
                audioSource.Play();
                paused = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
        active = false;
    }
}
