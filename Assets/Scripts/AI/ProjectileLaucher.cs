using UnityEngine;
using System.Collections;

public class ProjectileLauncher : NEO_Attack
{
    public Transform target;
    public Transform arm; // The arm to visually aim
    public float launchAngle; // in degrees
    public float gravity = 9.81f;
    public GameObject projectilePrefab;
    public float launchInterval = 2.0f; // Time interval between launches

    private void Start()
    {
        StartCoroutine(LaunchProjectilePeriodically());
    }

    void LaunchProjectile()
    {
        Vector3 targetPosition = target.position;
        Vector3 launchPosition = transform.position;
        Vector3 direction = targetPosition - launchPosition;
        float distance = new Vector2(direction.x, direction.z).magnitude;
        float yOffset = direction.y;

        float angleRad = launchAngle * Mathf.Deg2Rad;

        // Calculate initial velocity needed to hit the target
        float v0 = CalculateInitialVelocity(distance, yOffset, angleRad);

        // Determine velocity components
        Vector3 velocity = new Vector3(
            direction.normalized.x * v0 * Mathf.Cos(angleRad),
            v0 * Mathf.Sin(angleRad),
            direction.normalized.z * v0 * Mathf.Cos(angleRad)
        );

        // Instantiate the projectile and apply velocity
        GameObject projectile = Instantiate(projectilePrefab, launchPosition, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = velocity;

        // Aim the arm visually
        AimArm(velocity);
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
        arm.LookAt(apexPosition);
    }

    IEnumerator LaunchProjectilePeriodically()
    {
        while (baseSpawnAmount > 0)
        {
            LaunchProjectile();
            baseSpawnAmount--;
            yield return new WaitForSeconds(launchInterval);
        }
    }
}
