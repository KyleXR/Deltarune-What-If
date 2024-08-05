using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneAttack : NEO_Attack
{
    //public GameObject player;            // Reference to the player
    public Transform fireTransform;
    public GameObject attackPrefab;      // Reference to the attack prefab
    public GameObject altAttackPrefab;      // Reference to the alt attack prefab
    [Range(0, 1)] public float altAttackChance = 0.5f;
    public float spawnInterval = 1f;     // Time interval between spawns
    //public float attackSpeed = 10f;      // Speed of the attack
    private bool useAlt = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0f, 1f) <= altAttackChance)
        {
            useAlt = true;            
        }
        // Start the coroutine to spawn attacks
        StartCoroutine(ShootAttacks());
        transform.Rotate(Vector3.up, Random.Range(0, 359));
    }

    public override void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        base.InitializeAttack(handler, spawnTransform, targetTransform, currentUrgency);
    }

    // Coroutine to spawn and shoot attacks
    IEnumerator ShootAttacks()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(useAlt ? spawnInterval * 2 : spawnInterval);
            if (!useAlt)
            {
                // Create a ray from the attacker's position to the player's position
                Vector3 directionToPlayer = (targetTransform.position - fireTransform.position).normalized;
                Ray ray = new Ray(transform.position, directionToPlayer);

                // Instantiate the attack prefab at the attacker's position
                GameObject attack = Instantiate(attackPrefab, fireTransform.position, Quaternion.LookRotation(directionToPlayer));
                attack.transform.parent = transform.parent;
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
            }
            else
            {
                GameObject attack = Instantiate(altAttackPrefab, fireTransform.position, Quaternion.identity);
                attack.transform.parent = transform.parent;
                var soundBall = attack.GetComponent<SoundBall>();
                soundBall.cannonFire = false;
                soundBall.InitializeAttack(handler, fireTransform, targetTransform, urgency);
            }
        }
    }
}
