using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBall : NEO_Attack
{
    [SerializeField] private GameObject attackPrefab;

    public float pulseDuration = 1f;   // Duration of one pulse cycle
    public float maxScale = 2f;        // Maximum scale factor
    public float minScale = 1f;        // Minimum scale factor
    public float attackPulseScale = 4f; // Scale factor during attack pulse
    public int initialPulses = 4;
    public int spawnPulses = 2;
    public int attackAmount = 2;
    //public Vector3 spawnBounds;  // Add this to define the spawn bounds
    public float maxSpeed = 2f;  // Maximum speed of movement
    //public float decelerationTime = 1f;  // Time taken to decelerate to zero

    private bool isAttacking = false;
    private Vector3 originalScale = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        targetPosition = new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), Random.Range(-spawnBounds.y, spawnBounds.y) + 1, Random.Range(-spawnBounds.z, spawnBounds.z));

        // Start coroutines to pulse scale and lerp position
        StartCoroutine(PulseScale());
        StartCoroutine(SmoothMoveToTarget(targetPosition));
    }

    // Coroutine to pulse the scale of the GameObject
    IEnumerator PulseScale()
    {
        int currentPulses = 0;
        int attackCount = 0;

        yield return StartCoroutine(LerpScale(minScale, maxScale, pulseDuration * 2));

        while (attackCount < attackAmount)
        {
            // Scale up
            yield return StartCoroutine(LerpScale(minScale, maxScale, pulseDuration));

            // If it's time to spawn an attack
            if (currentPulses >= spawnPulses && isAttacking && attackCount < attackAmount)
            {
                // Additional pulse for attack spawn
                yield return StartCoroutine(LerpScale(maxScale, attackPulseScale, pulseDuration * 0.5f));
                SpawnAttacks(6);
                attackCount++;
                yield return StartCoroutine(LerpScale(attackPulseScale, maxScale, pulseDuration * 0.5f));
            }

            // Scale down
            yield return StartCoroutine(LerpScale(maxScale, minScale, pulseDuration));

            if (currentPulses >= initialPulses && !isAttacking)
            {
                isAttacking = true;
                currentPulses = 0;
            }

            if (currentPulses >= spawnPulses && isAttacking)
            {
                currentPulses = 0;
            }

            currentPulses++;
        }
        yield return StartCoroutine(LerpScale(minScale, 0, pulseDuration * 2));
        Destroy(this.gameObject);
    }

    IEnumerator LerpScale(float startScale, float endScale, float duration)
    {
        float currentScale = 0;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            currentScale = Mathf.Lerp(startScale, endScale, t / duration);
            transform.localScale = originalScale * currentScale;
            yield return null;
        }
        transform.localScale = originalScale * endScale; // Ensure the scale is set to the final value
    }

    // Coroutine to move the GameObject smoothly to the target position
    IEnumerator SmoothMoveToTarget(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float totalTime = distance / maxSpeed * 2;  // Time to travel the full distance at max speed

        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            // Calculate the fraction of time passed
            float t = elapsedTime / totalTime;

            // Calculate the current speed based on the remaining distance
            float currentSpeed = Mathf.Lerp(maxSpeed, 0, t);

            // Move the object towards the target position
            float step = currentSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Update elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final position is set to the target position
        transform.position = targetPosition;
    }




    // Method to spawn multiple attacks with evenly spread starting rotations
    void SpawnAttacks(int amount)
    {
        float angleStep = 360f / amount;
        float angle = 0f;

        for (int i = 0; i < amount; i++)
        {
            // Calculate the direction of the attack in local space
            float attackDirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float attackDirY = 0;
            float attackDirZ = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 attackDirectionLocal = new Vector3(attackDirX, attackDirY, attackDirZ);

            // Transform the local direction to world space
            Vector3 attackDirection = transform.TransformDirection(attackDirectionLocal);

            // Create a ray from the position in the calculated direction
            Ray ray = new Ray(transform.position, attackDirection);

            // Instantiate the attack prefab with the calculated rotation
            GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.LookRotation(attackDirection));

            // Make the attack follow the ray path if applicable
            attack.GetComponent<RayFollower>().FollowRayPath(ray, 5);

            // Increment the angle for the next attack
            angle += angleStep;
        }
        transform.Rotate(Vector3.up, angleStep * 0.5f);
    }
}
