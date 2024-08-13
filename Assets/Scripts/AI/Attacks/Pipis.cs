using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipis : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float coneAngle = 45f; // Angle of the cone in degrees
    public float raycastLength = 10f; // Length of the raycasts
    public int minRays = 5; // Minimum number of raycasts
    public int maxRays = 10; // Maximum number of raycasts
    public float forceMagnitude = 10f; // Magnitude of the force to be applied
    private Transform player; // Reference to the player object

    public GameObject miniHead;

    private Vector3 randomDirection;

    public List<string> ignoreTags;

    void Start()
    {
        // Generate a random direction vector
        randomDirection = Random.onUnitSphere;
        player = FindFirstObjectByType<FirstPersonController>().transform;
    }

    void Update()
    {
        // Rotate the game object in the random direction at the specified speed
        transform.Rotate(randomDirection * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ignoreTags.Contains(other.tag)) return;
        // Destroy the game object when it is triggered
        if (other.TryGetComponent<Attack>(out var attack)) return;
        ShootRays();
        Destroy(gameObject);
    }

    void ShootRays()
    {
        int numRays = Random.Range(minRays, maxRays + 1); // Random number of rays

        // Direction towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        for (int i = 0; i < numRays; i++)
        {
            // Generate a random direction within the cone
            Vector3 randomDir = Random.insideUnitSphere;
            float randomAngle = Random.Range(0, coneAngle);
            randomDir = Vector3.RotateTowards(directionToPlayer, randomDir, randomAngle * Mathf.Deg2Rad, 0.0f);

            // Instantiate the miniHead, set its rotation, and apply force to the Rigidbody
            GameObject miniHeadInstance = Instantiate(miniHead, transform.position, Quaternion.LookRotation(randomDir));
            Rigidbody rb = miniHeadInstance.GetComponent<Rigidbody>();
            rb.AddForce(randomDir * (forceMagnitude + Random.Range(-1f, 1f)), ForceMode.Impulse);
            rb.transform.parent = transform.parent;
            // Shoot a raycast
            //Ray ray = new Ray(transform.position, randomDir);
            //if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, LayerMask.GetMask("Player")))
            //{
            //    Debug.DrawRay(transform.position, randomDir * raycastLength, Color.red, 2.0f);
            //    // Handle hit detection (e.g., damage to player or other objects)
            //    Debug.Log("Hit: " + hit.collider.name);
            //}
            //else
            //{
            //    Debug.DrawRay(transform.position, randomDir * raycastLength, Color.green, 2.0f);
            //}
        }
    }
}
