using UnityEngine;
using System.Collections;

public class PipisLauncher : NEO_Attack
{
    public Transform target;
    //public Transform arm; // The arm to visually aim
    public float launchAngle; // in degrees
    public float gravity = 9.81f;
    public GameObject projectilePrefab;
    public float launchInterval = 2.0f; // Time interval between launches
    public float aimPreparationTime = 0.5f; // Time to aim before launching

    public float ranPositionOffsetMax = 5f;
    public float ranAngleOffsetMax = 5f;

    public bool applyRandomPositionOffset = false;
    public bool applyRandomAngleOffset = false;

    public float pulseDuration = 0.5f; // Duration for pulsing effect
    public float pulseScaleAmount = 1.5f; // Scale factor for pulsing effect
    public float scaleLerpDuration = 1.0f; // Duration to lerp scale to zero

    private Vector3 originalScale;
    private Vector3 currentTargetPosition;
    private Vector3 currentVelocity;

    private bool isFiring = false;
    private bool isScaling = false;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale
        StartCoroutine(ManageScaleCoroutine(originalScale, originalScale * pulseScaleAmount));
        StartCoroutine(LaunchProjectilePeriodically());
    }

    private void Update()
    {
        if (spawnTransform != null)
        {
            transform.position = spawnTransform.position;
        }
    }

    public override void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        base.InitializeAttack(handler, spawnTransform, targetTransform, currentUrgency);
    }

    void PrepareAim()
    {
        Vector3 targetPosition = targetTransform.position;
        Vector3 launchPosition = transform.position;

        // Apply random position offset if enabled
        if (applyRandomPositionOffset)
        {
            targetPosition += new Vector3(
                Random.Range(-ranPositionOffsetMax, ranPositionOffsetMax),
                Random.Range(-ranPositionOffsetMax, ranPositionOffsetMax),
                Random.Range(-ranPositionOffsetMax, ranPositionOffsetMax)
            );
        }

        Vector3 direction = targetPosition - launchPosition;
        float distance = new Vector2(direction.x, direction.z).magnitude;
        float yOffset = direction.y;

        float angleRad = launchAngle * Mathf.Deg2Rad;

        // Apply random angle offset if enabled
        if (applyRandomAngleOffset)
        {
            float randomAngleOffset = Random.Range(-ranAngleOffsetMax, ranAngleOffsetMax);
            angleRad += randomAngleOffset * Mathf.Deg2Rad;
        }

        // Calculate initial velocity needed to hit the target
        float v0 = CalculateInitialVelocity(distance, yOffset, angleRad);

        // Determine velocity components
        currentVelocity = new Vector3(
            direction.normalized.x * v0 * Mathf.Cos(angleRad),
            v0 * Mathf.Sin(angleRad),
            direction.normalized.z * v0 * Mathf.Cos(angleRad)
        );

        // Aim the arm visually
        AimArm(currentVelocity);
    }

    void LaunchProjectile()
    {
        // Instantiate the projectile and apply velocity
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = currentVelocity;
    }

    float CalculateInitialVelocity(float distance, float yOffset, float angleRad)
    {
        float term1 = gravity * distance * distance;
        float term2 = 2 * (yOffset - distance * Mathf.Tan(angleRad)) * Mathf.Pow(Mathf.Cos(angleRad), 2);
        float v0Squared = term1 / term2;
        return Mathf.Sqrt(Mathf.Abs(v0Squared)); // Ensure we take the positive square root
    }

    void AimArm(Vector3 velocity)
    {
        // Calculate the time to apex
        float timeToApex = velocity.y / gravity;
        Vector3 apexPosition = transform.position + velocity * timeToApex - 0.5f * Physics.gravity * timeToApex * timeToApex;

        // Aim the arm at the apex
        handler.UpdateCannonAim(apexPosition, false);
    }

    IEnumerator LaunchProjectilePeriodically()
    {
        isFiring = true;
        while (baseSpawnAmount > 0)
        {
            PrepareAim();
            yield return new WaitForSeconds(aimPreparationTime); // Wait for the preparation time
            LaunchProjectile();
            baseSpawnAmount--;
            yield return new WaitForSeconds(launchInterval);
        }
        isFiring = false;
        while(isScaling) yield return null;

        if (baseSpawnAmount <= 0)
        {
            handler.StopAiming();
            Destroy(gameObject);
        }
    }

    IEnumerator ManageScaleCoroutine(Vector3 startScale, Vector3 pulseScale)
    {
        isScaling = true;
        // Initial scale up from zero
        yield return LerpScaleCoroutine(Vector3.zero, startScale, scaleLerpDuration);

        while (isFiring)
        {
            yield return StartCoroutine(LerpScaleCoroutine(startScale, pulseScale, pulseDuration));
            yield return StartCoroutine(LerpScaleCoroutine(pulseScale, startScale, pulseDuration));
        }

        // Final scale down to zero
        yield return LerpScaleCoroutine(transform.localScale, Vector3.zero, scaleLerpDuration);
        isScaling = false;
    }

    IEnumerator LerpScaleCoroutine(Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set
        transform.localScale = toScale;
    }
}
