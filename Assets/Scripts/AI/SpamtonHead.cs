using PathCreation.Examples;
using UnityEngine;

public class SpamtonHead : MonoBehaviour
{
    [SerializeField] private GameObject visuals;
    private SpamtonHeadSpawner spawner;
    private PathFollower pathFollower;
    private Collider trigger;

    private void Start()
    {
        trigger = GetComponent<Collider>();
        trigger.enabled = false;
        visuals.SetActive(false);
    }
    public void Initialize(SpamtonHeadSpawner spawner, PathCreation.PathCreator pathCreator, float urgency = 0)
    {
        this.spawner = spawner;
        pathFollower = GetComponent<PathFollower>();
        pathFollower.pathCreator = pathCreator;

        pathFollower.speed += urgency * 0.1f;

        Invoke("Activate", 0.1f);
    }

    private void Activate()
    {
        trigger.enabled = true;
        visuals.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Ouchies");
            //Apply Damage
        }
        if (!other.CompareTag("Spamton"))
        {
            Destroy(gameObject);            
        }
    }
    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.RemoveDestroyedHead(this.gameObject);
        }
    }
}
