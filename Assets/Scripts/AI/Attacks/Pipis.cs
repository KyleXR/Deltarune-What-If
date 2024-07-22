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
    public RayFollower miniHeads;
    private Transform player; // Reference to the player object

    private Vector3 randomDirection;

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
        // Destroy the game object when it is triggered
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
            var miniHead = Instantiate(miniHeads, transform.position, Quaternion.LookRotation(randomDir));
            Ray ray = new Ray(transform.position, randomDir);
            miniHead.FollowRayPath(ray, raycastLength);
            // Shoot a raycast
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, LayerMask.NameToLayer("Player")))
            {
                Debug.DrawRay(transform.position, randomDir * raycastLength, Color.red, 2.0f);
                // Handle hit detection (e.g., damage to player or other objects)
                Debug.Log("Hit: " + hit.collider.name);
            }
            else
            {
                Debug.DrawRay(transform.position, randomDir * raycastLength, Color.green, 2.0f);
            }
        }
    }
}
