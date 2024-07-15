using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEO_AttackHandler : MonoBehaviour
{
    //[SerializeField] private GameObject spamtonHeadAttack;
    [SerializeField] private List<GameObject> attacks;
    [SerializeField] private ArmCannonAim cannonAim;
    [SerializeField] private Transform cannonTransform;
    public void SpawnRandomAttack(float urgency)
    {
        int ranId = Random.Range(0, attacks.Count);
        switch (ranId)
        {
            case 0:
                var spawnedAttack = Instantiate(attacks[ranId]);
                //TEMPORARY
                spawnedAttack.GetComponent<SpamtonHeadSpawner>().Initialize(urgency);
                break;
            case 1:
                cannonAim.EnableAiming(true);
                var laser = Instantiate(attacks[ranId], cannonTransform.position, cannonTransform.rotation);
                laser.GetComponent<LaserBeam>().InitializeLaser(cannonTransform, FindFirstObjectByType<FirstPersonController>().transform);
                break;
        }
        Debug.Log(ranId);
        
    }
}
