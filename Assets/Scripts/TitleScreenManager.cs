using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] SplineFollower cameraFollower;
    private void Start()
    {
        Debug.Log(cameraFollower.followSpeed);
        cameraFollower.RebuildImmediate();
    }
    public void OnPlay()
    {
        SceneManager.LoadScene("SampleMapScene");
        //Time.timeScale = 1.0f; //just in case
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}
