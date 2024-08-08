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

    // Counter to track how many times rotation has been paused
    private int pauseCounter = 0;

    // The current rotation speed used during rotation (allows pausing without losing the original speed)
    private float currentRotationSpeed;

    private void Start()
    {
        currentRotationSpeed = rotationSpeed;
    }

    void Update()
    {
        if (pauseCounter > 0)
        {
            isRotating = false;
            return; // Skip rotation if paused
        }

        if (rotateToTarget)
        {
            float currentRotation = transform.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);

            if (Mathf.Abs(angleDifference) > 0.01f)
            {
                isRotating = true;
                float rotationDirection = Mathf.Sign(angleDifference);
                float rotationAmount = currentRotationSpeed * Time.deltaTime;
                rotationAmount = Mathf.Min(rotationAmount, Mathf.Abs(angleDifference));
                transform.Rotate(Vector3.up, rotationAmount * rotationDirection);
            }
            else
            {
                isRotating = false;
            }
        }
        else
        {
            isRotating = true;
            float rotationAmount = currentRotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }
    }

    public void PauseRotation(float duration)
    {
        if (pauseCounter == 0)
        {
            currentRotationSpeed = rotationSpeed;
            rotationSpeed = 0; // Stop rotation
        }

        pauseCounter++;
        Invoke("ResumeRotation", duration);
    }

    private void ResumeRotation()
    {
        pauseCounter--;

        if (pauseCounter <= 0)
        {
            pauseCounter = 0; // Ensure counter doesn't go negative
            rotationSpeed = currentRotationSpeed;
            isRotating = true; // Resume rotation
        }
    }
}
