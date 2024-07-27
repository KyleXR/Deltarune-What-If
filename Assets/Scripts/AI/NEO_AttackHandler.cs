using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NEO_AttackHandler : MonoBehaviour
{
    //[SerializeField] private GameObject spamtonHeadAttack;
    private Animator animator;
    [SerializeField] private List<NEO_Attack> attacks;
    [SerializeField] private List<NEO_Attack> spawnedAttacks;
    [SerializeField] private ArmCannonAim cannonAim;
    [SerializeField] private Transform cannonTransform;
    [SerializeField] private Transform mouthTransform;
    private FirstPersonController player;
    private NEO_AI controller;
    private bool isAttacking = true;

    [SerializeField] private GameObject cannonModel;
    [SerializeField] private GameObject handModel;
    private bool isUsingCannon = false;
    private List<float> attackCooldowns = new List<float>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < attacks.Count; i++)
        {
            attacks[i].ID = i;
        }
        player = FindFirstObjectByType<FirstPersonController>();
        controller = GetComponent<NEO_AI>();
        ToggleCannon(true, false);
        foreach (var attack in attacks) attackCooldowns.Add(0);

        // Start the coroutine to update cooldown timers
        StartCoroutine(UpdateCooldowns());
    }

    private IEnumerator UpdateCooldowns()
    {
        while (true)
        {
            for (int i = 0; i < attackCooldowns.Count; i++)
            {
                if (attackCooldowns[i] > 0)
                {
                    attackCooldowns[i] -= Time.deltaTime;
                    if (attackCooldowns[i] < 0) attackCooldowns[i] = 0;
                }
            }
            yield return null;
        }
    }

    public void SpawnRandomAttack(float urgency)
    {
        if (isAttacking)
        {
            bool spawnedAttack = false;
            int attempts = 0;
            while (!spawnedAttack && attempts < 10)
            {
                attempts++;
                int ranId = Random.Range(0, attacks.Count);
                var attackType = attacks[ranId].attackType;
                switch (attackType)
                {
                    case NEO_Attack.AttackType.Cannon:
                        {
                            bool canSpawn = true;
                            if (attackCooldowns[ranId] > 0) canSpawn = false;
                            foreach (NEO_Attack attack in spawnedAttacks)
                            {
                                if (attack.attackType == NEO_Attack.AttackType.Cannon) canSpawn = false;

                                if (!canSpawn) break;
                            }
                            if (canSpawn)
                            {
                                controller.PauseRotation(5);
                                cannonAim.EnableAiming(true);
                                //cannonAim.playerAim = true;
                                var attack = Instantiate(attacks[ranId], cannonTransform.position, cannonTransform.rotation);
                                attack.InitializeAttack(this, cannonTransform, player.transform, urgency);
                                spawnedAttacks.Add(attack);
                                spawnedAttack = true;
                                ToggleCannon(true, true);

                                attackCooldowns[ranId] = attack.duration * 5;
                            }
                        }
                        break;
                    case NEO_Attack.AttackType.BulletPattern:
                        {
                            bool canSpawn = true;
                            if (attackCooldowns[ranId] > 0) canSpawn = false;
                            if (attacks[ranId].isUnique && canSpawn)
                            {
                                foreach (NEO_Attack attack in spawnedAttacks)
                                {
                                    if (attacks[ranId].ID == attack.ID)
                                    {
                                        canSpawn = false;
                                        break;
                                    }
                                }
                            }
                            if (canSpawn)
                            {
                                var attack = Instantiate(attacks[ranId], Vector3.zero, Quaternion.identity);
                                attack.InitializeAttack(this, attack.transform, player.transform, urgency);
                                spawnedAttacks.Add(attack);
                                spawnedAttack = true;
                                attackCooldowns[ranId] = attack.duration * 0.5f;
                            }
                        }
                        break;
                    case NEO_Attack.AttackType.Special:
                        {
                            bool canSpawn = true;
                            if (attackCooldowns[ranId] > 0) canSpawn = false;
                            foreach (NEO_Attack neoAttack in spawnedAttacks)
                            {
                                if (neoAttack.attackType == NEO_Attack.AttackType.Special) canSpawn = false;

                                if (!canSpawn) break;
                            }
                            // specil
                            if (canSpawn)
                            {
                                var attack = Instantiate(attacks[ranId], Vector3.zero, Quaternion.identity);
                                attack.InitializeAttack(this, attack.transform, player.transform, urgency);
                                if (attack.TryGetComponent<KromerVacuum>(out var kromerVacuum))
                                {
                                    Debug.Log("Kromer");
                                    kromerVacuum.SetTarget(mouthTransform);
                                    animator.SetBool("IsVacuuming", true);
                                    Invoke("StopVacuuming", kromerVacuum.duration + 2);
                                }
                                attackCooldowns[ranId] = attack.duration * 5;
                                spawnedAttack = true;
                                PauseAttack(attack.duration + 2);
                                controller.PauseRotation(attack.duration + 2);
                            }
                        }
                        break;
                }
            }
        }
    }

    public void RemoveAttack(NEO_Attack attack)
    {
        spawnedAttacks.Remove(attack);

        // Check if there are any other cannon attacks in the list
        bool hasOtherCannonAttacks = spawnedAttacks.Any(a => a.attackType == NEO_Attack.AttackType.Cannon);

        if (attack.attackType == NEO_Attack.AttackType.Cannon)
        {
            if (!hasOtherCannonAttacks)
            {
                cannonAim.EnableAiming(false);
                ToggleCannon(true, false);
            }
        }
    }

    public NEO_Attack SpawnDuplicateAttack(int id)
    {
        var attack = Instantiate(attacks[id], Vector3.zero, Quaternion.identity);
        spawnedAttacks.Add(attack);
        return attack;
    }

    public void UpdateCannonAim(Vector3 target, bool playerAim = true)
    {
        cannonAim.SetTargetTransform(target);
        cannonAim.playerAim = playerAim;
    }

    public void StopAiming()
    {
        cannonAim.EnableAiming(false);
    }

    public void ToggleCannon(bool set = false, bool setTo = false)
    {
        if (set) isUsingCannon = setTo;
        else isUsingCannon = !isUsingCannon;
        if (handModel != null) handModel.SetActive(!isUsingCannon);
        if (handModel != null) cannonModel.SetActive(isUsingCannon);
    }

    public void PauseAttack(float duration)
    {
        isAttacking = false;
        Invoke("UnpauseAttack", duration);
    }

    private void UnpauseAttack()
    {
        isAttacking = true;
    }

    private void StopVacuuming()
    {
        animator.SetBool("IsVacuuming", false);
    }
}
