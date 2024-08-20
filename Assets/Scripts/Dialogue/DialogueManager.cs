using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private List<DialogueVisualController> visualControllers;
    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log(dialogue.sentences[0]);
        //if (dialogue.portraitSprite.Count() > 0)
        //{
            visualControllers[0].gameObject.SetActive(true);
            visualControllers[0].StartDialogue(dialogue);
        //}
        //else if (dialogue.name != "")
        //{
        //    visualControllers[1].gameObject.SetActive(true);
        //    visualControllers[1].StartDialogue(dialogue);
        //}
        //else
        //{
        //    visualControllers[2].gameObject.SetActive(true);
        //    visualControllers[2].StartDialogue(dialogue);
        //}
    }
}
