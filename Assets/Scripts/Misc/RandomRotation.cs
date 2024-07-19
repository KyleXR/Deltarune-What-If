using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private Vector3 randomDirection;

    void Start()
    {
        // Generate a random direction vector
        randomDirection = Random.onUnitSphere;
    }

    void Update()
    {
        // Rotate the game object in the random direction at the specified speed
        transform.Rotate(randomDirection * rotationSpeed * Time.deltaTime);
    }
}
