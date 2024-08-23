using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogueTrigger))]
public class SpamTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider trigger;
    [SerializeField] GameObject spamtonNeo;
    [SerializeField] DialogueTrigger dialogueTrigger;
    [SerializeField] LevelTrigger levelTrigger;
    public IntroCarts carts;
    [SerializeField] Animator anim;

    [SerializeField] GameObject musicPlayer;
    private GameObject currentMusicPlayer;
    public AsyncOperation asyncLoad;

    private void Start()
    {
        spamtonNeo.SetActive(false);
        //dialogueTrigger = GetComponent<DialogueTrigger>();
        FindFirstObjectByType<DialogueVisualController>().dialogueEnd += StartCarts;
        FindFirstObjectByType<DialogueVisualController>().YoinkSpamton += YoinkSpamton;
    }

   


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            trigger.enabled = false;
            spamtonNeo.SetActive(true);
            dialogueTrigger.TriggerDialogue();
            FindFirstObjectByType<LookAtHandler>().LookAtNextTarget();
            StartCoroutine(LoadNextScene());
            currentMusicPlayer = Instantiate(musicPlayer);
        }
    }

    IEnumerator LoadNextScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // Check if the load is complete
            if (asyncLoad.progress >= 0.9f)
            {
                if(levelTrigger.isTriggered)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }
            yield return null;
        }
        Destroy(MusicManager.Instance.gameObject);
        yield return StartCoroutine(UnloadAndSetActiveScene());
    }
    IEnumerator UnloadAndSetActiveScene()
    {
        // After the original scene is unloaded, set the new scene as active
        Scene newScene = SceneManager.GetSceneByName("BattleScene");
        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
        }

        // Wait until the original scene is unloaded
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("SampleMapScene");
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

    }


    private void YoinkSpamton(bool yoink)
    {
        anim.enabled = true;
        anim.SetTrigger("Yoink");
    }

    private void StartCarts(bool start)
    {
        var player = FindAnyObjectByType<FirstPersonController>();
        if (player != null)
        {
            player.enabled = false;
        }
        FindFirstObjectByType<LookAtHandler>().LookAtNextTarget();
       
        carts.StartIntro();
        //spamtonNeo.SetActive(false);
        MusicManager.Instance.Stop();
        Destroy(currentMusicPlayer.gameObject);
        //currentMusicPlayer = Instantiate(musicPlayers[1]);
    }
}
