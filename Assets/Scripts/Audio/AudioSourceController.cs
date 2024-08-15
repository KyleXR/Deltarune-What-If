using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
		
	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		destroyTimer = GetComponent<DestroyTimer>();
		destroyTimer.enabled = false;
	}

		
	void LateUpdate()
	{
		if (active && !audioSource.isPlaying)
		{
			Stop();
		}

		if (parent != null) 
		{
			transform.position = parent.position;
		}

	}

	public void SetSourceProperties(AudioClip clip, float volume, float picth, bool loop, float spacialBlend)
	{
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.pitch = picth;
		audioSource.loop = loop;
		audioSource.spatialBlend = spacialBlend;

		if(type == AudioData.Type.SFX && !loop) 
		{
            destroyTimer.timeToDestroy = audioSource.clip.length;
            destroyTimer.enabled = true;
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

	public void Play()
	{
        active = true;
		if (audioSource != null)
		{
			audioSource.Play();
        }
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

			// Calculate the step size for volume reduction over time
			float step = startVolume / duration;

			while (audioSource.volume > 0)
			{
				audioSource.volume -= step * Time.deltaTime;
				yield return null;
			}

			// Ensure the volume is set to zero
			audioSource.volume = 0f;
		}
    }

    void OnApplicationFocus(bool hasFocus)
    {
		
        if (audioSource != null)
        {
            if (hasFocus)
            {
                //Debug.Log("UnPause");
				audioSource.time = playTime;
                audioSource.Play();
                //audioSource.UnPause();
            }
            else
            {
                //Debug.Log("Pause");
				playTime = audioSource.time;
                audioSource.Pause();
            }
        }
    }
    void OnApplicationPause(bool isPaused)
    {
        if (audioSource != null)
        {
            if (isPaused)
            {
                playTime = audioSource.time;
                audioSource.Pause();
            }
            else
            {
                //audioSource.UnPause();
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
