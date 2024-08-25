using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class CameraShakeDamage : MonoBehaviour
{
    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        playerHealth.OnTakeDamage += OnPlayerDamaged;
    }

    void OnPlayerDamaged(float damage)
    {
        // Get the player's max health
        float maxHealth = playerHealth.maxHealth;

        // Calculate the shake magnitude based on damage and max health
        float magnitude = Mathf.Clamp(damage / maxHealth, 0.1f, 1f) * 2; // Clamp to avoid too low or too high values

        // Optionally, you can scale the duration based on the magnitude
        float duration = 0.1f + magnitude * 0.4f; // Basic scaling of duration

        // Trigger the camera shake with the calculated magnitude and duration
        CameraShakeEvent.TriggerShake(duration, magnitude);
    }
}
