using UnityEngine;
using System.Collections;

public class NEO_AI : MonoBehaviour
{
    [SerializeField] RotationHandler rotator;
    [SerializeField] NEO_AttackHandler attackHandler;
    [SerializeField] Health health;
    [SerializeField] Animator animator;

    [SerializeField] float rotationDelay = 10;
    [SerializeField] float attackDelay = 5;
    [SerializeField] float urgency = 0;
    [SerializeField] float urgencyRate = .1f;
    [SerializeField] bool generateUrgency = true;

    [SerializeField] private float urgencyMultiplier = 0;

    void Start()
    {
        StartCoroutine(RotateAtRandomIntervals());
        StartCoroutine(AttackAtRandomIntervals());

        // Subscribe to the OnTakeDamage event
        FindFirstObjectByType<FirstPersonController>().GetComponent<Health>().OnTakeDamage += OnPlayerDamaged;

        PauseAttacks(13);
        PauseRotation(13);
        PauseAnimator(13);
        UrgencyCooldown(13);

        health.OnTakeDamage += ApplyUrgencyMultiplier;

        BIGSHOTENHANCEMENTS();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (health != null) health.OnTakeDamage -= OnPlayerDamaged;
    }

    private void OnPlayerDamaged(float damage)
    {
        Debug.Log("Player hurt!");
        UrgencyCooldown();
    }

    void Update()
    {
        if (generateUrgency && urgency <= 10)
        {
            float generatedUrgency = urgencyRate * Time.deltaTime;
            urgency += generatedUrgency + (generatedUrgency * urgencyMultiplier);
        }
    }
    private void ChangeRotation()
    {
        int newAngle = Random.Range(0, 8) * 45;
        rotator.targetRotation = newAngle;
        rotator.rotateToTarget = true;
    }

    public void PauseRotation(float duration)
    {
        rotator.PauseRotation(duration);
    }

    public void PauseAttacks(float duration)
    {
        attackHandler.PauseAttack(duration);
    }

    public void PauseAnimator(float duration)
    {
        animator.speed = 0;
        Invoke("UnpauseAnimator", duration);
    }

    private void UnpauseAnimator()
    {
        animator.speed = 1;
    }

    private IEnumerator RotateAtRandomIntervals()
    {
        while (true)
        {
            float tempUrgency = urgency;
            if (urgency >= rotationDelay * 0.75f) tempUrgency = rotationDelay * 0.75f;

            float waitTime = rotationDelay - tempUrgency;
            waitTime = Mathf.Max(waitTime, 0.1f);

            yield return new WaitForSeconds(waitTime);

            if (!rotator.isRotating) ChangeRotation();
        }
    }

    private IEnumerator AttackAtRandomIntervals()
    {
        while (true)
        {
            float waitTime = attackDelay - urgency;
            waitTime = Mathf.Max(waitTime, 1f);
            yield return new WaitForSeconds(waitTime);

            attackHandler.SpawnRandomAttack(urgency);
        }
    }

    public void UrgencyCooldown(float duration = 5)
    {
        urgency *= 0.25f;
        generateUrgency = false;
        Invoke("EnableUrgencyGeneration", duration);
    }
    public void ApplyUrgencyMultiplier(float damage)
    {
        float maxHealth = health.maxHealth;
        urgencyMultiplier += (damage / maxHealth);
    }


    public void EnableUrgencyGeneration()
    {
        generateUrgency = true;
    }

    private void BIGSHOTENHANCEMENTS()
    {
        health.maxHealth *= 1.977f * 4.99f;
        health.currentHealth = health.maxHealth;
    }
}
