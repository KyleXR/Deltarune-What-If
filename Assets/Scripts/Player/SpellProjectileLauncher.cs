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
    [SerializeField] private float SOULChargeTime = 1f;
    [SerializeField] private float SOULFireForce = 20f;

    [SerializeField] private List<GameObject> handObjects;
    [SerializeField] private GameObject SOULchargeFX;
    [SerializeField] private GameObject IceShockCastFX;
    private GameObject currentChargeFX;

    // Cooldown-related variables
    [SerializeField] private float spellCooldownTime = 0.25f; // Cooldown duration in seconds
    private float lastSpellCastTime;

    private float chargeStartTime;

    private void Start()
    {
        tension = GetComponent<TensionPoints>();
        lastSpellCastTime = -spellCooldownTime; // Initialize to allow immediate casting
    }

    void Update()
    {
        SwapHandObjects(tension.tensionPoints < tensionCosts[(int)targetingLogic.selectedSpell]);

        if (Input.GetMouseButtonDown(0))
        {
            chargeStartTime = Time.time;
        }
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Charging...");
            float chargeTime = Time.time - chargeStartTime;
            if (chargeTime > 0.1f && currentChargeFX == null && handObjects[0].activeInHierarchy)
            {
                //.Log("Charged");
                currentChargeFX = Instantiate(SOULchargeFX, handObjects[0].transform.position, Quaternion.identity);
                currentChargeFX.transform.parent = handObjects[0].transform;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            // Check if the cooldown has expired
            if (Time.time >= lastSpellCastTime + spellCooldownTime)
            {
                LaunchProjectile();
                if (currentChargeFX != null) { Destroy(currentChargeFX); }
                lastSpellCastTime = Time.time; // Update the last spell cast time
            }
            else
            {
                Debug.Log("Spell is on cooldown. Please wait.");
            }
        }
    }

    void LaunchProjectile()
    {
        Vector3 target = targetingLogic.FindCameraAimPosition();
        Vector3 direction;

        if (target != Vector3.zero)
        {
            direction = (target - launchPoint.position).normalized;
        }
        else
        {
            direction = (targetingLogic.playerCamera.transform.forward * 10f).normalized; // 10f is an example distance
        }

        GameObject projectile;

        if (targetingLogic.selectedSpell == TargetingLogic.Spell.SnowGrave && tension.tensionPoints >= tensionCosts[1] && targetingLogic.GetCurrentTarget() != null)
        {
            Vector3 spawnPosition = targetingLogic.aimerPos;
            RaycastHit hit;

            // Cast a ray downwards from the target position
            if (Physics.Raycast(spawnPosition + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                spawnPosition.y = hit.point.y;
            }
            else
            {
                spawnPosition.y -= groundCheckDistance;
            }
            projectile = Instantiate(spellPrefabs[1], spawnPosition, Quaternion.identity);
            var destroyTimer = projectile.GetComponent<DestroyTimer>();
            projectile.GetComponent<Attack>().damage = Mathf.Round((40 * Magic + 600 + Random.Range(0, 101)) / destroyTimer.timeToDestroy);

            tension.tensionPoints -= tensionCosts[1];
            projectile.transform.parent = transform.parent;
        }
        else if (targetingLogic.selectedSpell == TargetingLogic.Spell.IceShock && tension.tensionPoints >= tensionCosts[0])
        {
            // Adjustments for 3 projectiles within 5 degrees
            float angleOffset = 30f;

            // Create the base rotation (straight ahead)
            Quaternion baseRotation = Quaternion.LookRotation(direction);

            // Instantiate three projectiles with slight offsets
            for (int i = 0; i < 3; i++)
            {
                // Random vertical offset within -5 to 5 degrees
                float randomVerticalOffset = Random.Range(-angleOffset, angleOffset);

                // Calculate horizontal offset
                float horizontalOffset = (i - 1) * angleOffset; // -5, 0, 5 degrees for left, center, right

                // Combine the offsets into a final rotation
                Quaternion rotation = baseRotation * Quaternion.Euler(randomVerticalOffset, horizontalOffset, 0);
                projectile = Instantiate(spellPrefabs[0], launchPoint.position, rotation);

                HomingProjectile homingProjectile = projectile.GetComponent<HomingProjectile>();
                if (homingProjectile != null)
                {
                    homingProjectile.target = targetingLogic.GetCurrentTarget();
                }

                projectile.GetComponent<Attack>().damage = Mathf.Round(30 * (Magic - 10) + 90 + Random.Range(0, 11)) * 0.33f;
                projectile.transform.parent = transform.parent;
            }
            var fx = Instantiate(IceShockCastFX, launchPoint.position, Quaternion.identity);
            fx.transform.parent = launchPoint;
            tension.tensionPoints -= tensionCosts[0];
        }
        else if(targetingLogic.selectedSpell == TargetingLogic.Spell.HealPrayer && tension.tensionPoints >= tensionCosts[2])
        {
            projectile = Instantiate(spellPrefabs[4], transform.position - (Vector3.up * 0.5f), Quaternion.identity);
            projectile.GetComponent<HealPrayer>().healAmount = (int)Magic * 5;
            projectile.transform.parent = transform;
            tension.tensionPoints -= tensionCosts[2];
        }
        else
        {
            float chargeTime = Time.time - chargeStartTime;

            if (chargeTime >= SOULChargeTime)
            {
                projectile = Instantiate(spellPrefabs[3], launchPoint.position, Quaternion.LookRotation(direction));
            }
            else
            {
                projectile = Instantiate(spellPrefabs[2], launchPoint.position, Quaternion.LookRotation(direction));
            }
            projectile.GetComponent<Rigidbody>().AddForce(direction * SOULFireForce, ForceMode.Impulse);
            projectile.transform.parent = transform.parent;
        }

        
    }


    public void SwapHandObjects(bool soulMode = false)
    {
        handObjects[0].SetActive(soulMode);
        handObjects[1].SetActive(!soulMode && (int)targetingLogic.selectedSpell < 2);
        handObjects[2].SetActive(!soulMode && (int)targetingLogic.selectedSpell == 2);
    }
}
