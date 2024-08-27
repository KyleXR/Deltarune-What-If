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
        asyncLoad = SceneManager.LoadSceneAsync("MainBattleScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // Check if the load is complete
            if (asyncLoad.progress >= 0.9f)
            {
                if (levelTrigger.isTriggered)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }
            yield return null;
        }

        // Once the new scene is activated, unload the old scene
        StartCoroutine(UnloadAndSetActiveScene());
    }

    IEnumerator UnloadAndSetActiveScene()
    {
        // Set the new scene as active
        Scene newScene = SceneManager.GetSceneByName("MainBattleScene");
        while (!newScene.isLoaded)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(newScene);

        // Unload the original scene
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("SampleMapScene");
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("Finished");
    }

    private void YoinkSpamton(bool yoink)
    {
        anim.enabled = true;
        anim.SetTrigger("Yoink");
    }

    private void StartCarts(bool start)
    {
        MusicManager.Instance.Stop();
        Destroy(currentMusicPlayer.gameObject);
        Destroy(MusicManager.Instance.gameObject);

        var player = FindAnyObjectByType<FirstPersonController>();
        if (player != null)
        {
            player.enabled = false;
        }
        FindFirstObjectByType<LookAtHandler>().LookAtNextTarget();

        carts.StartIntro();
    }
}
