using UnityEngine;
using Dreamteck.Splines;
using System.Collections;

public class RaycastBounds : MonoBehaviour
{
    public float desiredMinDistance = 2f; // Desired minimum distance between carts
    public float speedAdjustmentRate = 0.1f; // Rate at which to adjust speed
    public float raycastInterval = 0.5f; // Interval in seconds for raycasting
    public SplineFollower[] carts;

    public Transform rayOnePosition; // Position for the first raycast
    public Transform rayTwoPosition; // Position for the second raycast

    void Start()
    {
        StartCoroutine(CartCheckCoroutine());
    }

    private IEnumerator CartCheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(raycastInterval);

            if (carts.Length < 2) continue;

            bool hitRight = ShootRay(rayOnePosition.position, rayOnePosition.right, carts[0], true);
            bool hitLeft = ShootRay(rayTwoPosition.position, rayTwoPosition.right, carts[2], false);

            // If no hits, check distances
            if (!hitRight && !hitLeft)
            {
                for (int i = 0; i < carts.Length - 1; i++)
                {
                    for (int j = i + 1; j < carts.Length; j++)
                    {
                        float distanceToCart = Vector3.Distance(carts[i].transform.position, carts[j].transform.position);

                        if (distanceToCart < desiredMinDistance)
                        {
                            AdjustCartSpeed(carts[j], true);
                        }
                        else if (distanceToCart > desiredMinDistance)
                        {
                            AdjustCartSpeed(carts[j], false);
                        }
                    }
                }
            }
        }
    }

    private bool ShootRay(Vector3 origin, Vector3 direction, SplineFollower cart, bool slowDown)
    {
        RaycastHit hit;
        float raycastDistance = desiredMinDistance;
        Ray ray = new Ray(origin, direction);

        Debug.DrawRay(ray.origin, ray.direction * desiredMinDistance, slowDown ? Color.yellow : Color.red);
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.transform == cart.transform)
            {
                AdjustCartSpeed(cart, slowDown);
                return true;
            }
        }
        return false;
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
