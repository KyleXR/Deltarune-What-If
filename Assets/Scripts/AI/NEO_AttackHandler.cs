using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEO_AttackHandler : MonoBehaviour
{
    //[SerializeField] private GameObject spamtonHeadAttack;
    [SerializeField] private List<GameObject> attacks;
    public void SpawnRandomAttack(float urgency)
    {
        int ranId = Random.Range(0, attacks.Count);
        Debug.Log(ranId);
        var spawnedAttack = Instantiate(attacks[ranId]);
        //TEMPORARY
        spawnedAttack.GetComponent<SpamtonHeadSpawner>().Initialize(urgency);
    }
}
