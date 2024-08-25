using UnityEngine;

public class CameraShakeEvent : MonoBehaviour
{
    // Define a delegate for the event
    public delegate void ShakeEvent(float duration, float magnitude);

    // Define the event based on the delegate
    public static event ShakeEvent OnShake;

    // Method to trigger the shake event
    public static void TriggerShake(float duration, float magnitude)
    {
        if (OnShake != null)
        {
            OnShake(duration, magnitude);
        }
    }
}
