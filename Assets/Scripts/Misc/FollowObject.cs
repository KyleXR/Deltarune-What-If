using System.Collections;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    // Reference to the target object
    public Transform target;
    public Vector3 offset = Vector3.zero;
    public float lerpSpeed = 5f;

    void Start()
    {
        // Start the coroutine to follow the target
        StartCoroutine(FollowTarget());
    }

    IEnumerator FollowTarget()
    {
        while (true)
        {
            // Check if the target is assigned
            if (target != null)
            {
                // Smoothly interpolate the position of this object to the target's position
                Vector3 targetPosition = target.position + offset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            }

            // Wait for the next frame
            yield return null;
        }
    }
}
