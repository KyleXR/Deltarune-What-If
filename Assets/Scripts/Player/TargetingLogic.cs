using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLogic : MonoBehaviour
{
    public string targetTag = "Target"; // The tag to identify target objects
    public LayerMask targetLayers; // The tag to identify target objects
    public float maxDistance = 20f; // Maximum distance to target
    public float maxAngle = 30f; // Maximum angle to target

    public GameObject snowgraveAim;
    public Vector3 aimerPos = Vector3.zero;

    public float groundCheckDistance = 20f; // Maximum distance to check for ground

    [HideInInspector] public Camera playerCamera;
    private Transform currentTarget;
    private GameObject tempTarget;

    
    [SerializeField] LayerMask groundLayer;

    public Spell selectedSpell = 0;
    public enum Spell
    {
        IceShock,
        SnowGrave
    }

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
            currentTarget = GetCurrentTarget();

            if (currentTarget != null)
            {
                //Debug.Log("Targeting: " + currentTarget.name);
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSpell = Spell.IceShock;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSpell = Spell.SnowGrave;
        }

        if (selectedSpell == Spell.SnowGrave && Input.GetMouseButton(1))
        {
            HandleSnowGraveAiming();
        }
        else
        {
            snowgraveAim.SetActive(false);
        }
    }

    void HandleSnowGraveAiming()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = aimerPos;
            RaycastHit hit;

            // Cast a ray downwards from the target position
            if (Physics.Raycast(aimerPos + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance, groundLayer))
            {
                // If ground is detected, set the Y position to the hit point
                targetPosition.y = hit.point.y;
                snowgraveAim.SetActive(true);
                snowgraveAim.transform.position = targetPosition;
            }
            else
            {
                // If no ground is detected, disable the aim indicator
                snowgraveAim.SetActive(false);
            }
        }
        else
        {
            snowgraveAim.SetActive(false);
        }
    }

    public Transform GetCurrentTarget()
    {
        switch (selectedSpell)
        {
            case Spell.IceShock:
                currentTarget = FindClosestTarget();
                break;
            case Spell.SnowGrave:
                currentTarget = FindCameraAim();
                break;
        }

        return currentTarget;
    }

    Transform FindCameraAim()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance, targetLayers))
        {

            aimerPos = hit.point;
            // Check if the hit object has the target tag
            // if (hit.collider.CompareTag(targetTag))
            {
                return hit.transform;
            }
        }
        return null;
    }
    public Vector3 FindCameraAimPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance, targetLayers))
        {
            {
                return hit.point;
            }
        }
        return Vector3.zero;
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
        //if (closestTarget == null) return FindCameraAim();
        return closestTarget;
    }
}
