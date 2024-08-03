using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnCollision : MonoBehaviour
{
    // Reference to the prefab that will be spawned
    public GameObject prefabToSpawn;

    // Offset for spawning the new object
    public Vector3 spawnOffset = Vector3.zero;

    // Booleans to control spawning on collision or trigger
    public bool spawnOnCollision = true;
    public bool spawnOnTrigger = false;

    // Update is called once per frame
    void Update()
    {

    }

    // This method is called when the collider attached to this object
    // collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        if (spawnOnCollision && prefabToSpawn != null)
        {
            // Calculate the spawn position
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the prefab at the calculated position
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    // This method is called when the collider attached to this object
    // enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (spawnOnTrigger && prefabToSpawn != null)
        {
            // Calculate the spawn position
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the prefab at the calculated position
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
