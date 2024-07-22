using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Rotation speed in degrees per second
    public float rotationSpeed = 10f;

    // Rotation axis (x, y, z)
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Calculate rotation for this frame
        float step = rotationSpeed * Time.deltaTime;

        // Apply rotation
        transform.Rotate(rotationAxis, step);
    }
}
