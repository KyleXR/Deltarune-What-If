using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensionPoints : MonoBehaviour
{
    public float tensionPoints = 0f; // Player's current TP
    public float maxTensionPoints = 100f; // Maximum TP
    public float grazeDistance = 1f; // Distance to trigger a graze
    public string enemyTag = "Spamton";
    public float checkInterval = 0.2f; // Time interval between each graze check

    void Start()
    {
        // Start the coroutine to check for grazing
        StartCoroutine(CheckForGrazesCoroutine());
    }

    IEnumerator CheckForGrazesCoroutine()
    {
        while (true)
        {
            CheckForGrazes();

            // Wait for the next interval before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CheckForGrazes()
    {
        // Find all objects tagged with the specified enemy tag
        GameObject[] attacks = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject attack in attacks)
        {
            Collider attackCollider = attack.GetComponent<Collider>();
            Attack attackComponent = attack.GetComponent<Attack>();
            if (attackCollider != null && attackComponent != null)
            {
                // Get the closest point on the collider to the player's position
                Vector3 closestPoint = attackCollider.ClosestPoint(transform.position);

                // Calculate the distance between the player and the closest point on the attack's collider
                float distance = Vector3.Distance(transform.position, closestPoint);

                // If within graze distance, add TP
                if (distance < grazeDistance)
                {
                    AddTensionPoints(1f); // Adjust the amount of TP gained per graze
                }
            }
        }
    }

    public void AddTensionPoints(float amount)
    {
        // Increase the TP, but clamp it to the maximum value
        tensionPoints = Mathf.Clamp(tensionPoints + amount, 0, maxTensionPoints);
    }
}
