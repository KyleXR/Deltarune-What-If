using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damageCooldown = 1f; // Cooldown time in seconds
    [SerializeField] private float currentHealth;
    private float lastDamageTime = -Mathf.Infinity; // Initialize to a time in the past

    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private bool spawnDamageNumber = false;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage (float damage)
    {
        TakeDamage(damage, Vector3.zero); //If the position doesn't matter.
    }
    public void TakeDamage(float damage, Vector3 damagePosition)
    {
        // Check if enough time has passed since the last damage was taken
        if (Time.time - lastDamageTime >= damageCooldown)
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
        }
    }

    void Die()
    {
        // Handle death (e.g., play death animation, deactivate the object)
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false); // Example action: deactivate the object
    }
}
