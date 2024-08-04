using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCartSpeedHandler : MonoBehaviour
{
    public Transform mainCart; // Reference to the main cart
    public List<SplineFollower> carts; // List of carts that need to be monitored
    public float distanceThreshold = 4f; // Distance threshold to check
    public float speedAdjustmentFactor = 0.1f; // Factor to control how quickly speed is adjusted
    public float maxSpeed = 10f; // Maximum speed limit
    public float minSpeed = 1f; // Minimum speed limit
    public float checkInterval = 1f; // Interval between distance checks
    public float proximityThreshold = 1f; // Distance within which carts should return to original speed faster

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
    }

    private IEnumerator CheckCartDistances()
    {
        while (true)
        {
            // Get the spline and the current positions
            SplineComputer spline = mainCart.GetComponent<SplineFollower>().spline;
            List<double> cartPositions = new List<double>();

            foreach (SplineFollower cart in carts)
            {
                if (cart != null)
                {
                    cartPositions.Add(cart.result.percent);
                }
                else
                {
                    cartPositions.Add(0);
                }
            }

            for (int i = 0; i < carts.Count; i++)
            {
                if (carts[i] == null) continue;

                SplineFollower currentCart = carts[i];
                double currentCartPosition = cartPositions[i];
                double targetCartPosition = i == 0 ? mainCart.GetComponent<SplineFollower>().result.percent : cartPositions[i - 1];

                // Calculate distance on the spline
                float distance = (float)(spline.CalculateLength(targetCartPosition, currentCartPosition));

                // Calculate the target speed adjustment
                float targetSpeedAdjustment = 0f;
                if (distance > distanceThreshold)
                {
                    targetSpeedAdjustment = speedAdjustmentFactor * (distance - distanceThreshold);
                }
                else if (distance <= distanceThreshold && distance > proximityThreshold)
                {
                    targetSpeedAdjustment = (originalSpeeds[currentCart] - currentCart.followSpeed) * 0.5f;
                }
                else if (distance <= proximityThreshold)
                {
                    targetSpeedAdjustment = (originalSpeeds[currentCart] - currentCart.followSpeed) * 1.0f;
                }

                // Smoothly adjust the cart speed
                AdjustCartSpeed(currentCart, targetSpeedAdjustment);
            }

            yield return new WaitForSeconds(checkInterval);
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
