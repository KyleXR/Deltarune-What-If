using UnityEngine;

public class ParentOnTrigger : MonoBehaviour
{
    public string targetTag = "Target"; // The tag of the objects you want to reparent

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the specified tag
        if (other.CompareTag(targetTag))
        {
            // Get the Transform of the object entering the trigger
            Transform objectTransform = other.transform;

            // Keep the object's world position
            Vector3 worldPosition = objectTransform.position;

            // Change the parent of the object to the owner of this trigger
            objectTransform.SetParent(transform);

            // Reset the object's position to its original world position
            objectTransform.position = worldPosition;
        }
    }
}
