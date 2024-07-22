using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpamtonHeadSpawner : NEO_Attack
{
    [SerializeField] private SpamtonHeadPathGenerator pathGenerator;
    [SerializeField] private GameObject spamtonHeadPrefab;
    [SerializeField] private float spawnDelay = 1.0f; // Time in seconds between spawns
    [SerializeField] private int spawnAmount = 5;

    public List<GameObject> spawnedHeads;
    public int overflowHeadAmount = -1; // use for when the amount of heads is over a set limit, and spawn the new amount in a new head spawner.
    private int totalSpawns = 0;
    private int spawnsLeft = 0;

    private void Start()
    {
        StartCoroutine(SpawnHeadsWithDelay());
    }

    public override void InitializeAttack(NEO_AttackHandler handler, Transform spawnTransform, Transform targetTransform, float currentUrgency = 0)
    {
        base.InitializeAttack(handler, spawnTransform, targetTransform, currentUrgency);
        spawnDelay -= urgency * 0.01f;
    }

    private IEnumerator SpawnHeadsWithDelay()
    {
        bool useHeadPool = false;
        int maxHeadCount = (int)(spawnAmount * 1.5f);
        if (overflowHeadAmount != -1)
        {
            useHeadPool = true;
            totalSpawns = maxHeadCount;
            overflowHeadAmount -= maxHeadCount;
        }
        if (totalSpawns <= 0) totalSpawns = (int)(spawnAmount + (urgency * 0.25));
        if (totalSpawns >= spawnAmount * 1.5f || overflowHeadAmount > 0)
        {
            var newSpawner = handler.SpawnDuplicateAttack(ID).GetComponent<SpamtonHeadSpawner>();
            if (!useHeadPool) newSpawner.overflowHeadAmount = totalSpawns - maxHeadCount;
            else newSpawner.overflowHeadAmount = overflowHeadAmount;
            newSpawner.InitializeAttack(handler, spawnTransform, targetTransform, urgency);
            totalSpawns = maxHeadCount;
        }
        spawnsLeft = totalSpawns;
        for (int i = 0; i < totalSpawns; i++) // Adjust the number of prefabs to spawn
        {
            spawnedHeads.Add(SpawnHead());
            spawnsLeft--;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public GameObject SpawnHead()
    {
        var head = Instantiate(spamtonHeadPrefab, pathGenerator.waypoints[0].transform.position, Quaternion.identity);
        head.GetComponent<SpamtonHead>().Initialize(this, pathGenerator.creator, urgency);
        head.GetComponent<SpamtonHeadPathFollower>().speed += urgency * 0.05f;
        if (head.TryGetComponent(out LookAtTarget look))
        {
            //Debug.Log(targetTransform.name);
            look.target = targetTransform;
        }
        return head;
    }

    public void RemoveDestroyedHead(GameObject spamtonHead)
    {
        spawnedHeads.Remove(spamtonHead.gameObject);
        if (spawnedHeads.Count == 0 && totalSpawns != 0)
        {
            Destroy(this.gameObject);
        }
    }
}
