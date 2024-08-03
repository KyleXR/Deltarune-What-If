using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // The target transform that this object will look at
    public Transform target;
    public bool lookAtPlayer = false;

    private void Start()
    {
        if (lookAtPlayer) target = FindFirstObjectByType<FirstPersonController>().transform;
    }

    void Update()
    {
        // Check if the target is set
        if (target != null)
        {
            // Rotate this transform to look at the target
            transform.LookAt(target);
        }
    }
}
