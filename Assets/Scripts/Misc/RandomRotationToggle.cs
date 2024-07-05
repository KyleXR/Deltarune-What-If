using UnityEngine;

public class RandomRotationToggle : MonoBehaviour
{
    public Transform targetTransform; // The transform to apply the rotations to
    public float switchInterval = 0.1f; // Interval between switching rotations
    private Quaternion originalRotation; // To store the original rotation
    private Quaternion randomRotation; // To store the random rotation
    public bool isJittering = false; // To keep track of the toggle state
    public bool toggle = false; // To keep track of the toggle state
    private Coroutine toggleCoroutine;
    private Animator animator;
    [SerializeField] private Rigidbody rb;

    void Start()
    {
        // Store the original rotation
        if (targetTransform != null)
        {
            originalRotation = targetTransform.rotation;
        }
        animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Toggle the rotation with the space key (you can change the key as needed)
        if (toggle)
        {
            if (isJittering)
            {
                StopJittering();
            }
            else
            {
                StartJittering();
            }
        }
    }

    void StartJittering()
    {
        if (targetTransform != null)
        {
            animator.enabled = false;
            // Generate a random rotation
            randomRotation = Random.rotation;
            // Start the coroutine to switch rotations
            toggleCoroutine = StartCoroutine(SwitchRotations());
            isJittering = true;
        }
    }

    void StopJittering()
    {
        if (toggleCoroutine != null)
        {
            animator.enabled = true;
            StopCoroutine(toggleCoroutine);
            // Reset to the original rotation
            targetTransform.rotation = originalRotation;
            isJittering = false;
            rb.velocity = Vector3.zero;
        }
    }

    System.Collections.IEnumerator SwitchRotations()
    {
        while (true)
        {
            // Switch between original and random rotation
            targetTransform.rotation = targetTransform.rotation == originalRotation ? randomRotation : originalRotation;
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
