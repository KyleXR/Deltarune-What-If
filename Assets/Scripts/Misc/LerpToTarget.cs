using System.Collections;
using UnityEngine;

public class LerpToTarget : MonoBehaviour
{
    public Transform targetTransform;
    public float duration = 1.0f;
    public float threshold = 0.1f; // Distance threshold to consider the target reached

    void Start()
    {
        if (targetTransform != null)
        {
            StartCoroutine(LerpToCurrentTargetPosition());
        }
    }

    IEnumerator LerpToCurrentTargetPosition()
    {
        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = targetTransform.position;
            float time = 0;

            while (time < duration)
            {
                if (targetTransform == null)
                {
                    yield break; // Stop if the targetTransform is null
                }

                targetPosition = targetTransform.position; // Update target position
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;

                // Check if the object is close enough to the target
                if (Vector3.Distance(transform.position, targetPosition) < threshold)
                {
                    transform.position = targetPosition; // Snap to the target position
                    Destroy(gameObject); // Destroy the object
                    yield break;
                }

                yield return null;
            }

            // Ensure the target position is set at the end of the current lerp cycle
            transform.position = targetPosition;
        }
    }
}
