using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpamtonHeadSpawner : MonoBehaviour
{
    [SerializeField] private SpamtonHeadPathGenerator pathGenerator;
    [SerializeField] private GameObject spamtonHeadPrefab;
    [SerializeField] private float spawnDelay = 1.0f; // Time in seconds between spawns
    [SerializeField] private int spawnAmount = 5;

    public List<GameObject> spawnedHeads;

    private float urgency = 0;
    private int totalSpawns = 0;

    private void Start()
    {
        StartCoroutine(SpawnHeadsWithDelay());
    }

    public void Initialize(float urgency)
    {
        this.urgency = urgency;
        spawnDelay -= urgency * 0.01f;
    }

    private IEnumerator SpawnHeadsWithDelay()
    {
        totalSpawns = (int)(spawnAmount + (urgency * 0.25));
        for (int i = 0; i < spawnAmount + (urgency * 0.25); i++) // Adjust the number of prefabs to spawn
        {
            spawnedHeads.Add(SpawnHead());
            totalSpawns--;
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public GameObject SpawnHead()
    {
        var head = Instantiate(spamtonHeadPrefab, pathGenerator.waypoints[0].transform.position, Quaternion.identity);
        head.GetComponent<SpamtonHead>().Initialize(this, pathGenerator.creator, urgency);
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
