using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTransform : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Transform resetPoint;

    private void Start()
    {
        if (target == null) target = FindFirstObjectByType<FirstPersonController>().gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
        {
            if (other.gameObject == target)
            {
                target.transform.position = resetPoint.position;
                target.transform.rotation = resetPoint.rotation;
            }
        }
    }
}
