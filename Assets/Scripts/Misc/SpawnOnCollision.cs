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

    public bool checkTag = false;

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
            if (checkTag && !collision.gameObject.CompareTag(tag))
            {
                SpawnPrefab(collision.transform);
            }
            else if(!checkTag)
            {
                SpawnPrefab(collision.transform);
            }
        }
    }

    // This method is called when the collider attached to this object
    // enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (spawnOnTrigger && prefabToSpawn != null)
        {
            if (checkTag && !other.CompareTag(tag))
            {
                SpawnPrefab(other.transform);
            }
            else if (!checkTag)
            {
                SpawnPrefab(other.transform);
            }
        }
    }
    private void SpawnPrefab(Transform parent)
    {
        // Calculate the spawn position
        Vector3 spawnPosition = transform.position + spawnOffset;

        // Instantiate the prefab at the calculated position
        var prefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        prefab.transform.parent = parent;
    }
}
