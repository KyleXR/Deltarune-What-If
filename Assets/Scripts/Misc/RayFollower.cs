using UnityEngine;

public class RayFollower : MonoBehaviour
{
    public float speed = 5f; // Speed at which the object follows the ray
    private Ray ray; // The ray to follow
    private Vector3 targetPosition; // The target position at the end of the ray
    private bool isFollowingRay = false;

    void Update()
    {
        if (isFollowingRay)
        {
            // Move the object towards the target position
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Check if the object has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                // Destroy the object once it reaches the end of the ray
                Destroy(gameObject);
            }
        }
    }

    // Call this method to start following the ray
    public void FollowRayPath(Ray ray, float length)
    {
        this.ray = ray;
        targetPosition = ray.origin + ray.direction * length;
        isFollowingRay = true;
        speed = Random.Range(speed - 2, speed + 2);
    }
}
