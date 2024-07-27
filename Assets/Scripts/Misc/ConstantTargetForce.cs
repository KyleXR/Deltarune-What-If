using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantTargetForce : MonoBehaviour
{
    private Rigidbody rb;
    public Transform targetTransform; // The target towards which the force is applied
    public float forceMultiplier = 10f; // Multiplier for the force applied
    public bool scaleForce = false; // Toggle for scaling force based on distance

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Check if targetTransform is assigned
        if (targetTransform != null)
        {
            // Calculate direction towards the target
            Vector3 direction = (targetTransform.position - transform.position).normalized;

            // Calculate distance to the target
            float distance = Vector3.Distance(transform.position, targetTransform.position);

            // Determine the force to be applied
            float force = forceMultiplier;

            // If scaleForce is true, scale the force based on the inverse of the distance
            if (scaleForce)
            {
                // Ensure we don't divide by zero
                force *= 1 / (distance + 0.1f); // Adding a small value to prevent division by zero
            }

            // Apply force in the calculated direction
            rb.AddForce(direction * force);
        }
    }
}
