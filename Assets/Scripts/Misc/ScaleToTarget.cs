using UnityEngine;

public class ScaleToTarget : MonoBehaviour
{
    public Transform targetTransform; // The target towards which the scaling effect is applied
    public float maxScale = 2f; // Maximum scale when close to the target
    public float minScale = 1f; // Minimum scale when far from the target
    public float scaleDistance = 10f; // Distance at which scaling reaches maxScale
    public float destroyScale = 0.1f;
    public bool destroyIfLesser = true;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale; // Store the initial scale of the object
    }

    void Update()
    {
        if (targetTransform != null)
        {
            // Calculate the distance between the object and the target
            float distance = Vector3.Distance(transform.position, targetTransform.position);

            // Calculate the scale factor based on the distance
            float scaleFactor = Mathf.Clamp01(distance / scaleDistance);

            // Interpolate between minScale and maxScale
            float newScale = Mathf.Lerp(minScale, maxScale, scaleFactor);

            // Apply the new scale to the object
            transform.localScale = initialScale * newScale;

            bool destroyObject = false;
            if (destroyScale > newScale && destroyIfLesser) destroyObject = true;
            else if (destroyScale < newScale && !destroyIfLesser) destroyObject = true;
            if (destroyScale < 0) destroyObject = false;

            if (destroyObject) Destroy(gameObject);
        }
    }
}
