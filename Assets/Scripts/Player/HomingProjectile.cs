using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public Transform target; // The target the projectile will home in on
    public float speed = 10f; // Speed of the projectile
    public float rotationSpeed = 5f; // Rotation speed of the projectile
    public float lifeTime = 5f; // Lifetime of the projectile before it gets destroyed
    public string targetTag = "Enemy"; // Tag of the targets to check for
    public float checkInterval = 0.5f; // Interval between checks for the closest target

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false; // Disable gravity for the projectile
        Destroy(gameObject, lifeTime); // Destroy the projectile after its lifetime has elapsed

        StartCoroutine(UpdateTarget());
    }

    // FixedUpdate is called once per physics update
    void FixedUpdate()
    {
        if (target != null)
        {
            // Calculate direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calculate rotation step
            float rotationStep = rotationSpeed * Time.fixedDeltaTime;

            // Rotate the projectile towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationStep);

            // Move the projectile forward
            rb.velocity = transform.forward * speed;
        }
        else
        {
            // If no target, just move forward
            rb.velocity = transform.forward * speed;
        }
    }

    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            FindClosestTarget();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTag);
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject potentialTarget in potentialTargets)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget < closestDistance && Vector3.Dot(transform.forward, directionToTarget.normalized) > 0)
            {
                closestDistance = distanceToTarget;
                closestTarget = potentialTarget.transform;
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget;
        }
    }
}
