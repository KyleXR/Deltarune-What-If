using Dreamteck.Splines.Primitives;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectileSpawner : MonoBehaviour
{
    public GameObject[] spellPrefabs;
    public Transform launchPoint;
    public TargetingLogic targetingLogic;
    public float groundCheckDistance = 100f; // Maximum distance to check for ground
    [SerializeField] LayerMask groundLayer;

    [SerializeField] List<float> tensionCosts;
    private TensionPoints tension;

    [SerializeField] private float Magic = 11;

    private void Start()
    {
        tension = GetComponent<TensionPoints>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Example: Left mouse button to launch
        {
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        Transform target = targetingLogic.GetCurrentTarget();

        if (target != null)
        {
            GameObject projectile;
            if (targetingLogic.selectedSpell == TargetingLogic.Spell.SnowGrave && tension.tensionPoints >= tensionCosts[1])
            {
                Vector3 spawnPosition = targetingLogic.aimerPos;
                RaycastHit hit;

                // Cast a ray downwards from the target position
                if (Physics.Raycast(spawnPosition + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance, groundLayer))
                {
                    // If ground is detected, set the Y position to the hit point
                    spawnPosition.y = hit.point.y;
                }
                else
                {
                    // If no ground is detected, set the Y position to the end of the raycast
                    spawnPosition.y -= groundCheckDistance;
                }
                projectile = Instantiate(spellPrefabs[1], spawnPosition, Quaternion.identity);
                var destroyTimer = projectile.GetComponent<DestroyTimer>();
                projectile.GetComponent<Attack>().damage = Mathf.Round((40 * Magic + 600 + Random.Range(0, 101))/destroyTimer.timeToDestroy*0.1f);

                tension.tensionPoints -= tensionCosts[1];
            }
            else if (targetingLogic.selectedSpell == TargetingLogic.Spell.IceShock && tension.tensionPoints >= tensionCosts[0])
            {
                projectile = Instantiate(spellPrefabs[0], launchPoint.position, launchPoint.rotation);
                HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();

                if (homingProjectile != null)
                {
                    homingProjectile.target = target;
                }
                projectile.GetComponent<Attack>().damage = Mathf.Round(30 * (Magic - 10) + 90 + Random.Range(0, 11));
                tension.tensionPoints -= tensionCosts[0];
            }
            else
            {
                projectile = Instantiate(spellPrefabs[0], launchPoint.position, launchPoint.rotation);
            }
            projectile.transform.parent = transform.parent;
        }
    }
}
