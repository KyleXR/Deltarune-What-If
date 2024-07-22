using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneAttack : NEO_Attack
{
    //public GameObject player;            // Reference to the player
    public Transform fireTransform;
    public GameObject attackPrefab;      // Reference to the attack prefab
    public float spawnInterval = 1f;     // Time interval between spawns
    //public float attackSpeed = 10f;      // Speed of the attack

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = FindFirstObjectByType<FirstPersonController>().transform; // temp
        // Start the coroutine to spawn attacks
        StartCoroutine(ShootAttacks());
        transform.Rotate(Vector3.up, Random.Range(0, 359));
    }

    // Coroutine to spawn and shoot attacks
    IEnumerator ShootAttacks()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(spawnInterval);

            // Create a ray from the attacker's position to the player's position
            Vector3 directionToPlayer = (targetTransform.position - fireTransform.position).normalized;
            Ray ray = new Ray(transform.position, directionToPlayer);

            // Instantiate the attack prefab at the attacker's position
            GameObject attack = Instantiate(attackPrefab, fireTransform.position, Quaternion.LookRotation(directionToPlayer));
            float distance = 30;
            RaycastHit hit;
            int layerMask = 1 << 0;
            if (Physics.Raycast(fireTransform.position, directionToPlayer, out hit, distance, layerMask))
            {
                distance = hit.distance;
                //Debug.DrawRay(fireTransform.position, directionToPlayer * distance, Color.red, 1);
            }
            //Debug.Log(distance);
            //Debug.DrawRay(fireTransform.position, directionToPlayer * distance, Color.yellow, 1);
            attack.GetComponent<RayFollower>().FollowRayPath(ray, distance);
            //// Set the velocity of the attack to move towards the player
            //Rigidbody rb = attack.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.velocity = directionToPlayer * attackSpeed;
            //}
        }
    }
}
