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

	[SerializeField, Range(0, 1)] private float volume = 1;
	[SerializeField, Range(0, 0.2f)] private float volumeRandom = 0;
	[SerializeField, Range(-24, 24)] private float pitch = 0;
	[SerializeField, Range(0, 12)] private float pitchRandom = 0;
	[SerializeField, Range(0, 1)] private float spacialBlend = 1;
	[SerializeField] private bool loop = false;
	[SerializeField] private Type type = Type.SFX;
	[SerializeField, Range(0f, 1f)] private float chance = 1;

	
	public AudioSourceController Play(Transform parent)
	{
		AudioSourceController controller = Play(parent.position);
		controller.SetParent(parent);

		return controller;
	}

	public AudioSourceController Play(Vector3 position)
	{
        float rand = Random.Range(0, 100) * 0.01f;
        AudioSourceController controller = AudioManager.Instance.GetController(type);
		float volume = this.volume + Random.Range(-volumeRandom, volumeRandom);
		float pitch = AudioUtilities.SemitoneToPitch(this.pitch + Random.Range(-pitchRandom, pitchRandom));
		controller.SetSourceProperties(GetAudioClip(), volume, pitch, loop, spacialBlend);
		controller.SetPosition(position);
        if (rand <= chance) controller.Play();

		return controller;
	}


	private AudioClip GetAudioClip()
	{
		if (audioClips.Length == 0) return null;

		int index = (audioClips.Length == 1) ? 0 : Random.Range(0, audioClips.Length);
		return audioClips[index];
	}
}
