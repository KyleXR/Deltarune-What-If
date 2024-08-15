using UnityEngine;
using System.Collections;

public class NEO_AI : MonoBehaviour
{
    [SerializeField] RotationHandler rotator;
    [SerializeField] NEO_AttackHandler attackHandler;
    [SerializeField] Health health;

    [SerializeField] float rotationDelay = 10;
    [SerializeField] float attackDelay = 5;
    [SerializeField] float urgency = 0;
    [SerializeField] float urgencyRate = .1f;
    [SerializeField] bool generateUrgency = true;

    void Start()
    {
        StartCoroutine(RotateAtRandomIntervals());
        StartCoroutine(AttackAtRandomIntervals());

        // Subscribe to the OnTakeDamage event
        FindFirstObjectByType<FirstPersonController>().GetComponent<Health>().OnTakeDamage += OnPlayerDamaged;
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
        if (generateUrgency && urgency <= 10) urgency += urgencyRate * Time.deltaTime;
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

    public void UrgencyCooldown()
    {
        urgency *= 0.5f;
        generateUrgency = false;
        Invoke("EnableUrgencyGeneration", 5);
    }

    public void EnableUrgencyGeneration()
    {
        generateUrgency = true;
    }
}
