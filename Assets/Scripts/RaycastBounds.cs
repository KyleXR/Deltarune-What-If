using UnityEngine;
using Dreamteck.Splines;

public class RaycastBounds : MonoBehaviour
{
    public float desiredMinDistance = 2f; // Desired minimum distance between carts
    public float speedAdjustmentRate = 0.1f; // Rate at which to adjust speed
    public float raycastInterval = 0.5f; // Interval in seconds for raycasting
    private float raycastTimer;
    public SplineFollower[] carts;

    public Transform rayOnePosition; // Position for the first raycast
    public Transform rayTwoPosition; // Position for the second raycast

    void Update()
    {
        raycastTimer += Time.deltaTime;
        if (raycastTimer >= raycastInterval)
        {
            raycastTimer = 0f;
            CheckCartDistances();
        }
    }

    public void CheckCartDistances()
    {
        if (carts.Length < 3) return;

        SplineFollower secondCart = carts[1];
        SplineFollower firstCart = carts[0];
        SplineFollower thirdCart = carts[2];

        RaycastHit hit;
        float raycastDistance = desiredMinDistance;

        bool hitRight = false, hitLeft = false;

        Ray backRay = new Ray(rayOnePosition.position, rayOnePosition.right);

        Debug.DrawRay(backRay.origin, backRay.direction, Color.yellow);
        // Raycast to the right from rayOnePosition
        if (Physics.Raycast(backRay, out hit, raycastDistance))
        {
            if (hit.transform == firstCart.transform)
            {
                AdjustCartSpeed(firstCart, true);
                hitRight = true;
            }
            else if (hit.transform == thirdCart.transform)
            {
                //Debug.DrawRay(backRay.origin, backRay.direction, Color.yellow);
                AdjustCartSpeed(thirdCart, true);
                hitRight = true;
            }
        }

        Ray frontRay = new Ray(rayTwoPosition.position, rayTwoPosition.right);

        Debug.DrawRay(frontRay.origin, frontRay.direction, Color.red);
        // Raycast to the left from rayTwoPosition
        if (Physics.Raycast(frontRay, out hit, raycastDistance))
        {
            if (hit.transform == firstCart.transform)
            {
                AdjustCartSpeed(firstCart, false);
                hitLeft = true;
            }
            else if (hit.transform == thirdCart.transform)
            {
                //Debug.DrawRay(frontRay.origin, frontRay.direction, Color.red);
                AdjustCartSpeed(thirdCart, false);
                hitLeft = true;
            }
        }

        // If no hits, check distances
        if (!hitRight && !hitLeft)
        {
            float distanceToFirstCart = Vector3.Distance(secondCart.transform.position, firstCart.transform.position);
            float distanceToThirdCart = Vector3.Distance(secondCart.transform.position, thirdCart.transform.position);

            if (distanceToFirstCart < desiredMinDistance)
            {
                AdjustCartSpeed(firstCart, true);
            }
            else if (distanceToFirstCart > desiredMinDistance)
            {
                AdjustCartSpeed(firstCart, false);
            }

            if (distanceToThirdCart < desiredMinDistance)
            {
                AdjustCartSpeed(thirdCart, true);
            }
            else if (distanceToThirdCart > desiredMinDistance)
            {
                AdjustCartSpeed(thirdCart, false);
            }
        }
    }

    private void AdjustCartSpeed(SplineFollower cart, bool slowDown)
    {
        if (slowDown)
        {
            cart.followSpeed = Mathf.Max(cart.followSpeed - speedAdjustmentRate, 0.1f);
        }
        else
        {
            cart.followSpeed += speedAdjustmentRate;
        }
    }
}
