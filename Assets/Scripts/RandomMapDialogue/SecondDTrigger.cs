using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondDTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider trigger;
    [SerializeField] DialogueTrigger dialogueTrigger;
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite[] portraitSprite;
    private int timesTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            switch(timesTriggered)
            {
                case 0:
                    dialogueTrigger.TriggerDialogue();
                    timesTriggered++;
                    break;
                case 1:
                    dialogueTrigger.dialogue.sentences[0] = sentences[0];
                    dialogueTrigger.dialogue.portraitSprite[0] = portraitSprite[0];
                    dialogueTrigger.TriggerDialogue();
                    timesTriggered++;
                    break;
                case 2:
                    dialogueTrigger.dialogue.sentences[0] = sentences[1];
                    dialogueTrigger.dialogue.portraitSprite[0] = portraitSprite[1];
                    dialogueTrigger.TriggerDialogue();
                    timesTriggered++;
                    break;
                case 3:
                    dialogueTrigger.dialogue.sentences[0] = sentences[2];
                    dialogueTrigger.dialogue.portraitSprite[0] = portraitSprite[2];
                    dialogueTrigger.TriggerDialogue();
                    timesTriggered++;
                    break;
                case 4:
                    dialogueTrigger.dialogue.sentences[0] = sentences[3];
                    dialogueTrigger.dialogue.portraitSprite[0] = portraitSprite[3];
                    dialogueTrigger.TriggerDialogue();
                    trigger.enabled = false;
                    break;
                default:
                    break;
            }
            
            
        }
    }
}
