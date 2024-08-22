using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdDTrigger : MonoBehaviour
{
    [SerializeField] BoxCollider trigger;
    [SerializeField] DialogueTrigger dialogueTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            dialogueTrigger.TriggerDialogue();
            trigger.enabled = false;
        }
    }
}
