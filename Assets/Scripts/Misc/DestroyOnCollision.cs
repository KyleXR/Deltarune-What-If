using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public bool checkTagOnCollide = true;
    public List<string> ignoreTags;

    private void OnTriggerEnter(Collider other)
    {
        if (ignoreTags.Contains(other.tag)) return;
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
        if (ignoreTags.Contains(collision.gameObject.tag)) return;
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
