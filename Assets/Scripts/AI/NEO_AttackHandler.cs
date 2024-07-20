using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEO_AttackHandler : MonoBehaviour
{
    //[SerializeField] private GameObject spamtonHeadAttack;
    [SerializeField] private List<NEO_Attack> attacks;
    [SerializeField] private List<NEO_Attack> spawnedAttacks;
    [SerializeField] private ArmCannonAim cannonAim;
    [SerializeField] private Transform cannonTransform;
    private FirstPersonController player;
    private NEO_AI controller;

    [SerializeField] private GameObject cannonModel;
    [SerializeField] private GameObject handModel;
    private bool isUsingCannon = false;

    private void Start()
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            attacks[i].ID = i;
        }
        player = FindFirstObjectByType<FirstPersonController>();
        controller = GetComponent<NEO_AI>();
        ToggleCannon(true, false);
    }
    public void SpawnRandomAttack(float urgency)
    {
        bool spawnedAttack = false;
        while (!spawnedAttack)
        {
            int ranId = Random.Range(0, attacks.Count);
            if (attacks[ranId].attackType == NEO_Attack.AttackType.Cannon)
            {
                bool canSpawn = true;
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
                }
            }
            if (attacks[ranId].attackType == NEO_Attack.AttackType.BulletPattern)
            {
                bool canSpawn = true;
                if (attacks[ranId].isUnique)
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
                }
            }
        }
        //switch (ranId)
        //{
        //    case 0:
        //        var spawnedAttack = Instantiate(attacks[ranId]);
        //        //TEMPORARY
        //        spawnedAttack.GetComponent<SpamtonHeadSpawner>().Initialize(urgency);
        //        break;
        //    case 1:
        //        cannonAim.EnableAiming(true);
        //        var laser = Instantiate(attacks[ranId], cannonTransform.position, cannonTransform.rotation);
        //        laser.GetComponent<LaserBeam>().InitializeLaser(cannonTransform, FindFirstObjectByType<FirstPersonController>().transform);
        //        break;
        //}
    }
    public void RemoveAttack(NEO_Attack attack)
    {
        if (attack.attackType == NEO_Attack.AttackType.Cannon)
        {
            cannonAim.EnableAiming(false);
            ToggleCannon(true, false);
        }
        spawnedAttacks.Remove(attack);
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
}
