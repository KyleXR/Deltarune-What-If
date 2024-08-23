using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Canvas gameOverScreen;
    [SerializeField] Image heartLeft;
    [SerializeField] Image heartMid;
    [SerializeField] Image heartRight;
    [SerializeField] Image cover;
    [SerializeField] ButtonHighlighter continueBtn;
    [SerializeField] ButtonHighlighter giveUpBtn;
    [SerializeField] Camera GOCam;


    void Start()
    {
        gameOverScreen.enabled = false;
        GOCam.gameObject.SetActive(false);
       
        FindFirstObjectByType<FirstPersonController>().GetComponent<Health>().OnDeath += SetActive;
    }

    // Update is called once per frame
    void Update()
    {
        EnableHeart();
    }

    public void SetActive(bool isActive)
    {
        Time.timeScale = 0f;
        gameOverScreen.enabled = true;
        GOCam.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        heartLeft.enabled = false;
        heartMid.enabled = true;
        heartRight.enabled = false;
        cover.enabled = false;
    }

    public void EnableHeart()
    {
        if(continueBtn.IsHighlighted())
        {
            heartLeft.enabled = true;
            heartMid.enabled = false;
            heartRight.enabled = false;
        }
        else if(giveUpBtn.IsHighlighted())
        {
            heartLeft.enabled = false;
            heartMid.enabled = false;
            heartRight.enabled = true;
        }
        else
        {
            heartLeft.enabled = false;
            heartMid.enabled = true;
            heartRight.enabled = false;
        }
    }

    public void Continue()
    {
        
        SceneManager.LoadScene("KyleSceneBackup");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f; 
        cover.enabled = true;
        //gameOverScreen.enabled = false;
        //GOCam.gameObject.SetActive(false);
    }



    public void GiveUp()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
