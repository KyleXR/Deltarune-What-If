using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPrayer : MonoBehaviour
{
    [SerializeField] GameObject healSoundFX;
    [SerializeField] public int healAmount;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("HealPlayer", 0.5f);
    }

    private void HealPlayer()
    {
        Instantiate(healSoundFX);
        FindFirstObjectByType<FirstPersonController>().GetComponent<Health>().Heal(healAmount);
    }
}
