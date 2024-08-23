using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LookAtHandler : MonoBehaviour
{
    public bool lookAt = false;
    [SerializeField] Transform[] targets;
    [SerializeField] Camera mainCamera;
    private Transform currentTarget;
    public float rotationSpeed = 5.0f;
    private int lookAtId = -1;

    public void LookAtNextTarget()
    {
        lookAtId++;
        if (lookAtId >= targets.Length) lookAtId = targets.Length - 1;
        currentTarget = targets[lookAtId];
        lookAt = true;
        StartCoroutine(TurnCamera());

        Debug.Log("LookAtId: " + lookAtId); 
    }

    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private IEnumerator TurnCamera()
    {
        while (lookAt)
        {

            // Smoothly rotate the camera to look at the target
            Vector3 direction = currentTarget.position - mainCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the camera has reached the target rotation
            if (Quaternion.Angle(mainCamera.transform.rotation, targetRotation) < 0.1f)
            {
                // Break out of the loop
                lookAt = false;
            }

            yield return null;
        }
    }
}
