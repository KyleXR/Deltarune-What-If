using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTrigger : MonoBehaviour
{
    SpamTrigger trigger;
    public bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.UnloadSceneAsync("SampleMapScene");
        isTriggered = true;
        MusicManager.Instance.Stop();
    }
}
