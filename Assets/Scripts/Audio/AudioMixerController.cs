using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioMixerController", menuName = "Audio/AudioMixerController")]
public class AudioMixerController : ScriptableObject
{
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField, Range(0, 1)] private float volumeMaster = 1;
	[SerializeField, Range(0, 1)] private float volumeSFX = 1;
	[SerializeField, Range(0, 1)] private float volumeMusic = 1;

	// audio mixer controls
	const string MASTER_VOLUME = "MasterVolume";
	const string SFX_VOLUME = "SFXVolume";
	const string MUSIC_VOLUME = "MusicVolume";

	private void OnValidate()
	{
		masterVolume = volumeMaster;
		sfxVolume = volumeSFX;
		musicVolume = volumeMusic;
	}

	public float masterVolume
	{
		get
		{
			audioMixer.GetFloat(MASTER_VOLUME, out float dB);
			return AudioUtilities.DecibelToLinear(dB);
		}
		set
		{
			float dB = AudioUtilities.LinearToDecibel(value);
			audioMixer.SetFloat(MASTER_VOLUME, dB);
		}
	}

	public float sfxVolume
	{
		get
		{
			audioMixer.GetFloat(SFX_VOLUME, out float dB);
			return AudioUtilities.DecibelToLinear(dB);
		}
		set
		{
			float dB = AudioUtilities.LinearToDecibel(value);
			audioMixer.SetFloat(SFX_VOLUME, dB);
		}
	}

	public float musicVolume
	{
		get
		{
			audioMixer.GetFloat(MUSIC_VOLUME, out float dB);
			return AudioUtilities.DecibelToLinear(dB);
		}
		set
		{
			float dB = AudioUtilities.LinearToDecibel(value);
			audioMixer.SetFloat(MUSIC_VOLUME, dB);
		}
	}

}
