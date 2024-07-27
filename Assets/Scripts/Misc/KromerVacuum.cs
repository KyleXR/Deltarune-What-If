using System.Collections;
using UnityEngine;

public class KromerVacuum : NEO_Attack
{
    public GameObject objectToSpawn;

    public float minSpawnRadius = 1f; // Minimum distance from the origin
    public float spawnRadius = 5f; // Maximum distance from the origin
    public float spawnAngle = 45f; // Cone angle in degrees
    public float spawnInterval = 1f; // Time between spawns
    //public float spawnDuration = 10f; // Total duration to spawn objects

    private void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate a random angle within the cone
            float randomAngle = Random.Range(-spawnAngle / 2f, spawnAngle / 2f);

            // Convert the angle to a direction
            Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * targetTransform.forward;

            // Calculate a random distance within the min and max radius
            float distance = Random.Range(minSpawnRadius, spawnRadius);

            // Calculate the spawn position
            Vector3 spawnPosition = targetTransform.position + direction * distance;

            // Spawn the object
            var kromer = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            if (kromer.TryGetComponent<ConstantTargetForce>(out var targetForce))
            {
                targetForce.targetTransform = targetTransform;
            }
            if (kromer.TryGetComponent<ScaleToTarget>(out var scaleToTarget))
            {
                scaleToTarget.targetTransform = targetTransform;
            }
            // Wait for the next spawn
            yield return new WaitForSeconds(spawnInterval);

            // Update elapsed time
            elapsedTime += spawnInterval;
        }
    }
}
