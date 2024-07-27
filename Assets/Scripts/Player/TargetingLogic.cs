using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLogic : MonoBehaviour
{
    public string targetTag = "Target"; // The tag to identify target objects
    public float maxDistance = 20f; // Maximum distance to target
    public float maxAngle = 30f; // Maximum angle to target

    private Camera playerCamera;
    private Transform currentTarget;
    private GameObject tempTarget;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main; // Assuming the main camera is the player's camera
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)) // Check if the right mouse button is held down
        {
            currentTarget = FindClosestTarget();
            if (currentTarget != null)
            {
                Debug.Log("Targeting: " + currentTarget.name);
                // Additional logic when a target is found
            }
        }
        else
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            Debug.DrawLine(playerCamera.transform.position, currentTarget.position, Color.red);
        }
    }

    public Transform GetCurrentTarget()
    {
        currentTarget = FindClosestTarget();
        //if (currentTarget == null)
        //{
        //    // Create or reuse the temporary target GameObject
        //    if (tempTarget == null)
        //    {
        //        tempTarget = new GameObject("TempTarget");
        //        // Add a collider and set it as a trigger
        //        Collider collider = tempTarget.AddComponent<SphereCollider>();
        //        collider.isTrigger = true;
        //        tempTarget.tag = targetTag;
        //    }
        //    tempTarget.transform.position = playerCamera.transform.position + playerCamera.transform.forward * maxDistance;
        //    tempTarget.transform.rotation = playerCamera.transform.rotation;
        //    currentTarget = tempTarget.transform;
        //}
        return currentTarget;
    }

    Transform FindClosestTarget()
    {
        Transform closestTarget = null;
        float closestAngle = maxAngle;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(targetTag))
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance <= maxDistance)
            {
                Vector3 directionToTarget = obj.transform.position - playerCamera.transform.position;
                float angle = Vector3.Angle(playerCamera.transform.forward, directionToTarget);

                if (angle <= maxAngle && angle < closestAngle)
                {
                    closestAngle = angle;
                    closestTarget = obj.transform;
                }
            }
        }

        return closestTarget;
    }
}
