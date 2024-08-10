using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damageCooldown = 1f; // Cooldown time in seconds
    [SerializeField] public float currentHealth;
    private float lastDamageTime = -Mathf.Infinity; // Initialize to a time in the past

    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private bool spawnDamageNumber = false;

    public event Action<float> OnTakeDamage;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage (float damage, float cooldownOverride = -1)
    {
        TakeDamage(damage, Vector3.zero, cooldownOverride); //If the position doesn't matter.
    }
    public void TakeDamage(float damage, Vector3 damagePosition, float cooldownOverride = -1)
    {
        // Check if enough time has passed since the last damage was taken
        if (Time.time - lastDamageTime >= ((cooldownOverride < 0) ? damageCooldown : cooldownOverride))
        {
            if (spawnDamageNumber && damageNumberPrefab != null)
            {
                if (damagePosition == Vector3.zero) damagePosition = transform.position;
                var damageNumber = Instantiate(damageNumberPrefab, damagePosition, Quaternion.identity).GetComponent<DamageNumber>();
                damageNumber.Initialize((int)damage);
            }
            currentHealth -= damage;
            lastDamageTime = Time.time; // Update the last damage time

            // Ensure health doesn't drop below zero
            if (currentHealth <= 0)
            {
                Die();
            }
            OnTakeDamage?.Invoke(damage); //Calls the event
        }
    }

    void Die()
    {
        // Handle death (e.g., play death animation, deactivate the object)
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false); // Example action: deactivate the object
    }
}
