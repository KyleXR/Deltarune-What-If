using PathCreation.Examples;
using UnityEngine;
using System.Collections;

public class SpamtonHead : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool fireProjectile = false;
    [SerializeField] private float fireRate = 2f; // Time between projectile shots

    [SerializeField] private GameObject visuals;
    private SpamtonHeadSpawner spawner;
    private SpamtonHeadPathFollower pathFollower;
    private Collider trigger;

    private void Start()
    {
        trigger = GetComponent<Collider>();
        trigger.enabled = false;
        visuals.SetActive(false);

        if (fireProjectile)
        {
            fireRate += Random.Range(-0.5f, 2f);
            if (Random.Range(0f, 1f) <= 0.75f) StartCoroutine(FireProjectiles());
        }
    }

    public void Initialize(SpamtonHeadSpawner spawner, PathCreation.PathCreator pathCreator, float urgency = 0)
    {
        this.spawner = spawner;
        pathFollower = GetComponent<SpamtonHeadPathFollower>();
        pathFollower.pathCreator = pathCreator;

        pathFollower.speed += urgency * 0.1f;

        Invoke("Activate", 0.2f);
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

        // Stop the coroutine if the object is destroyed
        StopCoroutine(FireProjectiles());
    }

    private IEnumerator FireProjectiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            // Optionally, add some code here to initialize the projectile, set its direction, etc.

            // Example: If your projectile has a Rigidbody, you might want to set its velocity
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * 10f; // Adjust the speed as needed
            }
            projectile.transform.parent = transform.parent;
        }
    }
}
