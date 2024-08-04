using Dreamteck.Splines;
using UnityEngine;

public class CartFollowerManager : MonoBehaviour
{
    public SplineFollower mainCartFollower;
    public SplineFollower[] additionalCartFollowers;
    public float distanceBetweenCarts = 2.0f; // Adjust this to set the desired distance between carts
    private double mainCartPosition;

    public void InitializeCarts()
    {
        if (mainCartFollower != null)
        {
            mainCartFollower.SetDistance(distanceBetweenCarts * additionalCartFollowers.Length);

        }

        // Get the current position of the main cart on the spline
        double mainCartPosition = mainCartFollower.result.percent;
        double splineLength = mainCartFollower.spline.CalculateLength();

        // Calculate the position for each additional cart
        for (int i = 0; i < additionalCartFollowers.Length; i++)
        {
            // Calculate the distance in percent on the spline
            double distanceInPercent = distanceBetweenCarts / splineLength;
            // Calculate the new position for the additional cart
            double newCartPosition = mainCartPosition - ((i + 1) * distanceInPercent);

            // Ensure the position wraps around correctly if it goes below 0
            if (newCartPosition < 0) newCartPosition += 1;

            // Set the new position
            additionalCartFollowers[i].SetPercent(newCartPosition);
        }
    }

    private void Update()
    {
        for (int i = 0; i < additionalCartFollowers.Length; i++)
        {
            additionalCartFollowers[i].followSpeed = mainCartFollower.followSpeed; 
        }
    }
}
