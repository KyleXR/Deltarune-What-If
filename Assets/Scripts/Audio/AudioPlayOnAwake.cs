using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayOnAwake : MonoBehaviour
{
	public AudioData audioData;
	[Range(0f, 1f)] public float chance = 1;
	//[SerializeField] private bool setControlerParent = false;

	private AudioSourceController controller;
	void Awake()
	{
        if (Application.isPlaying)
		{
			//Debug.Log("I HAFT AWOKENED!!!! -" + name);
			float rand = Random.Range(0, 100) * 0.01f;
			//Debug.Log(rand.ToString());
			if (audioData != null && rand <= chance)
			{
				controller = audioData.Play(transform);
				if (controller != null ) { controller.transform.parent = gameObject.transform; }
			}
		}
	}

    private void OnDestroy()
    {
		if (controller != null)
		{
			if (Application.isPlaying)controller.Stop();
			Destroy(controller.gameObject);
		}
        if (Application.isPlaying) Destroy(gameObject);
    }
}
