using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartSpeedHandler : MonoBehaviour
{
    public Transform mainCart; // Reference to the main cart
    public List<SplineFollower> carts; // List of carts that need to be monitored
    public float distanceThreshold = 4f; // Distance threshold to check
    public float speedAdjustmentFactor = 0.1f; // Factor to control how quickly speed is adjusted
    public float maxSpeed = 10f; // Maximum speed limit
    public float minSpeed = 1f; // Minimum speed limit
    public float checkInterval = 1f; // Interval between distance checks
    public float proximityThreshold = 1f; // Distance within which carts should return to original speed faster
    public float fluctuationRange = 2f; // Range for speed fluctuation
    public float fluctuationInterval = 5f; // Interval to update speed fluctuation

    private Dictionary<SplineFollower, float> originalSpeeds = new Dictionary<SplineFollower, float>();
    private float targetSpeed;
    private float originalSpeed;

    private void Start()
    {
        // Record the original speed for each cart
        foreach (SplineFollower cart in carts)
        {
            if (cart != null)
            {
                originalSpeeds[cart] = cart.followSpeed;
            }
        }

        // Initialize the target speed for the main cart
        originalSpeed = mainCart.GetComponent<SplineFollower>().followSpeed;
        targetSpeed = originalSpeed;

        // Start the coroutines
        StartCoroutine(CheckCartDistances());
        StartCoroutine(UpdateSpeedFluctuation());
    }

    private IEnumerator CheckCartDistances()
    {
        while (true)
        {
            foreach (SplineFollower cart in carts)
            {
                if (cart == null) continue;

                Vector3 cartPosition = cart.transform.position;
                float distance = Vector3.Distance(mainCart.position, cartPosition);

                // Determine the direction from the main cart to the cart
                Vector3 directionToCart = (cartPosition - mainCart.position).normalized;

                // Use the forward vector of the main cart to determine if the cart is in front or behind
                float dotProduct = Vector3.Dot(mainCart.forward, directionToCart);

                // Calculate the target speed adjustment
                float targetSpeedAdjustment = 0f;
                if (distance > distanceThreshold)
                {
                    // Cart is too far, adjust speed proportionally
                    if (dotProduct > 0) // Cart is in front of the main cart
                    {
                        targetSpeedAdjustment = -speedAdjustmentFactor * (distance - distanceThreshold);
                    }
                    else // Cart is behind the main cart
                    {
                        targetSpeedAdjustment = speedAdjustmentFactor * (distance - distanceThreshold);
                    }
                }
                else if (distance <= distanceThreshold && distance > proximityThreshold)
                {
                    // Gradually return to the original speed as it approaches the threshold
                    targetSpeedAdjustment = (originalSpeeds[cart] - cart.followSpeed) * 0.5f;
                }
                else if (distance <= proximityThreshold)
                {
                    // Very close to the threshold, quickly return to original speed
                    targetSpeedAdjustment = (originalSpeeds[cart] - cart.followSpeed) * 1.0f;
                }

                // Smoothly adjust the cart speed
                AdjustCartSpeed(cart, targetSpeedAdjustment);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator UpdateSpeedFluctuation()
    {
        while (true)
        {
            // Randomly set a new target speed around the original speed
            targetSpeed = originalSpeed + Random.Range(-fluctuationRange, fluctuationRange);

            yield return new WaitForSeconds(fluctuationInterval);
        }
    }

    private void AdjustCartSpeed(SplineFollower cart, float targetAdjustment)
    {
        if (cart != null)
        {
            // Calculate the new speed
            float originalSpeed = originalSpeeds[cart];
            float newSpeed = Mathf.Clamp(cart.followSpeed + targetAdjustment, minSpeed, maxSpeed);

            // Smoothly adjust the speed to avoid overshooting
            cart.followSpeed = Mathf.Lerp(cart.followSpeed, newSpeed, 0.1f);
        }
    }

    private void Update()
    {
        // Smoothly transition the main cart speed towards the target speed
        SplineFollower mainCartFollower = mainCart.GetComponent<SplineFollower>();
        if (mainCartFollower != null)
        {
            mainCartFollower.followSpeed = Mathf.Lerp(mainCartFollower.followSpeed, targetSpeed, 0.1f);
        }
    }
}
