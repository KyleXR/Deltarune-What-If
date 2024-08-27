using UnityEngine;

public class SnowHandler : MonoBehaviour
{
    [SerializeField] private Health spamHealth;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float maxEmissionRate = 100f;
    [SerializeField] private float minEmissionRate = 0f;

    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        if (spamHealth == null)
        {
            Debug.LogError("Boss Health reference not set.");
            return;
        }

        if (particles == null)
        {
            Debug.LogError("Particle System reference not set.");
            return;
        }

        emissionModule = particles.emission;
        spamHealth.OnTakeDamage += OnSpamTakeDamage;
    }

    private void OnSpamTakeDamage(float damage)
    {
        // Calculate the emission rate based on inverse of current health percentage
        float healthPercentage = spamHealth.currentHealth / spamHealth.maxHealth;
        float emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, 1f - healthPercentage);

        // Update the particle system's emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to avoid memory leaks
        if (spamHealth != null)
        {
            spamHealth.OnTakeDamage -= OnSpamTakeDamage;
        }
    }
}
