using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSourceController audioSourceSFX;
    [SerializeField] private AudioSourceController audioSourceMusic;

    private List<AudioSourceController> audioSourceControllers = new List<AudioSourceController>();

    private void Start()
    {
        FindFirstObjectByType<FirstPersonController>().GetComponent<Health>().OnDeath += StopAllSounds;
    }

    public AudioSourceController GetController(AudioData.Type type)
    {
        AudioSourceController output = null;

        if (audioSourceControllers.Count > 0)
        {
            output = audioSourceControllers.Find(controller => controller.type == type);
            if (output != null)
            {
                audioSourceControllers.Remove(output);
                return output;
            }
        }

        switch (type)
        {
            case AudioData.Type.SFX:
                return Instantiate(audioSourceSFX);
            case AudioData.Type.MUSIC:
                return Instantiate(audioSourceMusic);
            default:
                return null;
        }
    }

    public void ReturnController(AudioSourceController controller)
    {
        if (!audioSourceControllers.Contains(controller))
        {
            audioSourceControllers.Add(controller);
        }
    }

    private void OnApplicationQuit()
    {
        StopAllSounds(true);
    }

    private void StopAllSounds(bool yes)
    {
        foreach (var controller in audioSourceControllers)
        {
            if (controller != null)
            {
                controller.Stop();
                if (controller.gameObject != null) Destroy(controller.gameObject);
            }
        }
        audioSourceControllers.Clear();
    }

}
