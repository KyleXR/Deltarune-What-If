using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceTrigger : MonoBehaviour
{
    public float force = 10;
    public Vector3 direction = Vector3.up;
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Rigidbody>().AddForce(force * direction, ForceMode.Impulse);
    }
}
