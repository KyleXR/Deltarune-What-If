using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public bool checkTagOnCollide = true;
    private void OnTriggerEnter(Collider other)
    {
        if (checkTagOnCollide && !other.gameObject.CompareTag(tag)) Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (checkTagOnCollide && !collision.gameObject.CompareTag(tag)) Destroy(gameObject);
    }
}
