using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public bool checkTagOnCollide = true;
    private void OnTriggerEnter(Collider other)
    {
        
        if (checkTagOnCollide && !other.gameObject.CompareTag(tag))
        {
            Debug.Log(other.name);
            Destroy(gameObject);
        }
        else if (!checkTagOnCollide)
        {
            Debug.Log(other.name);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (checkTagOnCollide && !collision.gameObject.CompareTag(tag))
        {
            Debug.Log(collision.gameObject.name);
            Destroy(gameObject);
        }
        else if (!checkTagOnCollide)
        {
            Debug.Log(collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}
