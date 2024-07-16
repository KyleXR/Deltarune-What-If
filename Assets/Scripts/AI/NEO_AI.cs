using UnityEngine;
using System.Collections;

public class NEO_AI : MonoBehaviour
{
    [SerializeField] RotationHandler rotator;
    [SerializeField] NEO_AttackHandler attackHandler;

    [SerializeField] float health = 1997;
    [SerializeField] float rotationDelay = 5;
    [SerializeField] float urgency = 0;
    [SerializeField] float urgencyRate = .1f;
    [SerializeField] bool generateUrgency = true;

    void Start()
    {
        StartCoroutine(RotateAtRandomIntervals());
        StartCoroutine(AttackAtRandomIntervals());
    }

    // Update is called once per frame
    void Update()
    {
        if (urgency <= rotationDelay * 0.75f && generateUrgency) urgency += urgencyRate * Time.deltaTime;
    }

    private void ChangeRotation()
    {
        // Get new target rotation at 45 degree increments
        int newAngle = Random.Range(0, 8) * 45;
        rotator.targetRotation = newAngle;
        rotator.rotateToTarget = true; // Make sure to rotate to the new target
    }

    public void PauseRotation(float duration)
    {
        rotator.PauseRotation(duration);
    }

    private IEnumerator RotateAtRandomIntervals()
    {
        while (true)
        {
            // Wait for a random time based on rotationDelay and urgency
            float waitTime = rotationDelay - urgency;
            waitTime = Mathf.Max(waitTime, 0.1f); // Ensure we don't get a negative or zero wait time

            yield return new WaitForSeconds(waitTime);

            // Change rotation after the wait
            if (!rotator.isRotating) ChangeRotation();
        }
    }

    private IEnumerator AttackAtRandomIntervals()
    {
        while (true)
        {
            float waitTime = rotationDelay - urgency;
            waitTime = Mathf.Max(waitTime, 0.01f);
            yield return new WaitForSeconds(waitTime);

            attackHandler.SpawnRandomAttack(urgency);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0; //initiate death
        }
        UrgencyCooldown();
    }

    public void EnableUrgencyGeneration()
    {
        generateUrgency = true;
    }

    public void UrgencyCooldown()
    {
        //Will be called when Spamton NEO damages the player
        urgency *= 0.5f;
        generateUrgency = false;
        Invoke("EnableUrgencyGeneration", 5);
    }
}