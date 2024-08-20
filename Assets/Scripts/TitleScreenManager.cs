using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void OnPlay()
    {
        SceneManager.LoadScene("SampleMapScene");
        Time.timeScale = 1.0f; //just in case
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}
