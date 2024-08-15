using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtilities
{
	private static float twelfthRootOfTwo = Mathf.Pow(2.0f, 1.0f / 12.0f);

	public static float SemitoneToPitch(float semitone)
	{
		return Mathf.Clamp(Mathf.Pow(twelfthRootOfTwo, semitone), 0.0f, 4.0f);
	}

	public static float PitchToSemitone(float pitch)
	{
		return Mathf.Log(pitch, twelfthRootOfTwo);
	}

	public static float DecibelToLinear(float dB)
	{
		return (dB > -80) ? Mathf.Clamp01(Mathf.Pow(10.0f, dB / 20.0f)) : 0;
	}
	public static float LinearToDecibel(float linear)
	{
		return (linear > 0) ? Mathf.Clamp(20.0f * Mathf.Log10(linear), -80.0f, 0.0f) : -80.0f;
	}
}
