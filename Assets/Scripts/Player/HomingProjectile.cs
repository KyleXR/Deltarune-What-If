using UnityEngine;
using System.Collections;

public class HomingProjectile : MonoBehaviour
{
    public LayerMask targetLayer;  // Set this in the Inspector
    public float speed = 5f;
    public float rotationSpeed = 200f;
    public float targetSearchInterval = 0.5f;  // Interval in seconds between target searches
    public Transform target;

    void Start()
    {
        StartCoroutine(FindTargetCoroutine());
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    IEnumerator FindTargetCoroutine()
    {
        while (true)
        {
            FindTarget();
            yield return new WaitForSeconds(targetSearchInterval);
        }
    }

    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, Mathf.Infinity, targetLayer);
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = collider.transform;
            }
        }
    }
}
