using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensionOnCollision : MonoBehaviour
{
    public bool checkTag = false;
    public List<string> ignoreTags;

    public float tensionGain = 1f;

    private TensionPoints tension;

    private void Start()
    {
        tension = FindFirstObjectByType<TensionPoints>();
    }

    // This method is called when the collider attached to this object
    // collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        if (ignoreTags.Contains(collision.gameObject.tag)) return;
        if (checkTag && !collision.gameObject.CompareTag(tag))
        {
            GiveTension();
        }
        else if (!checkTag)
        {
            GiveTension();
        }
    }

    // This method is called when the collider attached to this object
    // enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (ignoreTags.Contains(other.tag)) return;
        if (checkTag && !other.CompareTag(tag))
        {
            GiveTension();
        }
        else if (!checkTag)
        {
            GiveTension();
        }
    }
    private void GiveTension()
    {
        tension.AddTensionPoints(tensionGain);
    }
}
