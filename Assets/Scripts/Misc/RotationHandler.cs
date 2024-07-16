using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 45.0f;

    // Target rotation in degrees (used only if `rotateToTarget` is true)
    public float targetRotation = 90.0f;

    // Toggle between continuous rotation and rotating to a target
    public bool rotateToTarget = false;

    // Flag to indicate if rotation is in progress
    public bool isRotating = false;

    private float originalSpeed = 0;

    // Update is called once per frame
    void Update()
    {
        if (rotateToTarget)
        {
            // Calculate the shortest angle difference between current and target rotations
            float currentRotation = transform.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);

            // If the angle difference is small enough, stop rotating
            if (Mathf.Abs(angleDifference) > 0.01f)
            {
                // Indicate that rotation is in progress
                isRotating = true;

                // Determine the direction to rotate
                float rotationDirection = Mathf.Sign(angleDifference);

                // Calculate rotation for this frame
                float rotationAmount = rotationSpeed * Time.deltaTime;

                // Ensure we don't overshoot the target
                rotationAmount = Mathf.Min(rotationAmount, Mathf.Abs(angleDifference));

                // Apply rotation to the object
                transform.Rotate(Vector3.up, rotationAmount * rotationDirection);
            }
            else
            {
                // Rotation is complete
                isRotating = false;
            }
        }
        else
        {
            // Continuous rotation
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }
    }
    public void PauseRotation(float duration)
    {
        originalSpeed = rotationSpeed;
        rotationSpeed = 0;
        Invoke("StartRotation", duration);
    }
    private void StartRotation()
    {
        rotationSpeed = originalSpeed;
    }
}
