using System.Collections.Generic;
using UnityEngine;

public class ApplyForceOnCollision : MonoBehaviour
{
    [SerializeField] float forceMagnitude = 10f;
    [SerializeField] Vector2 forceRange = Vector2.up;
    [SerializeField] ForceMode forceMode = ForceMode.Impulse;

    [SerializeField] bool randomDirection = false;

    [SerializeField] bool forceOnCollision = true;
    [SerializeField] bool forceOnTrigger = false;

    [SerializeField] bool checkTag = false;
    [SerializeField] List<string> ignoreTags;

    private void OnCollisionEnter(Collision collision)
    {
        if (forceOnCollision) ApplyForce(collision.contacts[0].point, collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (forceOnCollision) ApplyForce(collision.contacts[0].point, collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (forceOnTrigger) ApplyForce(transform.position, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (forceOnTrigger) ApplyForce(transform.position, other);
    }

    private void ApplyForce(Vector3 contactPoint, Collider otherCollider)
    {
        if (checkTag && ignoreTags.Contains(otherCollider.tag)) return;
        Debug.Log("Force");

        Rigidbody rb = otherCollider.attachedRigidbody;
        if (rb != null)
        {
            Vector3 forceDirection;
            if (randomDirection)
            {
                forceDirection = Random.insideUnitSphere;
            }
            else
            {
                forceDirection = (otherCollider.transform.position - contactPoint).normalized;
            }

            float randomForce = forceMagnitude + Random.Range(forceRange.x, forceRange.y);
            rb.AddForce(forceDirection * randomForce, forceMode);
        }
    }
}
