using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTargeter : MonoBehaviour
{
    private Transform[] points;
    [SerializeField] SpamtonHeadPathGenerator pathGenerator;
    public Transform playerTarget;

    public Vector3 rangeMin = Vector3.one*-10;
    public Vector3 rangeMax = Vector3.one*10;
    public bool isArc = false;
    public float arcMidModifier = 0.5f;

    private void Start()
    {
        points = pathGenerator.waypoints;
        if (!isArc) TargetPlayer();
        else ArcPath();
        pathGenerator.waypoints = points;
        pathGenerator.GeneratePath();
    }
    public void TargetPlayer()
    {
        points[0].position = GenerateRandomPosition();
        //points[1].position = GenerateRandomPosition();
        if (playerTarget == null) playerTarget = FindFirstObjectByType<FirstPersonController>().transform;
        if (playerTarget == null) playerTarget.position = GenerateRandomPosition();
        points[1].position = playerTarget.position;
        points[2].position = GenerateRandomPosition();
    }
    public void ArcPath()
    {
        //Debug.Log("Blue");
        var ran = Random.Range(0, 4);
        if (points.Length >= 3)
        {
            switch (ran)
            {
                case 0:
                    points[0].position = new Vector3(rangeMin.x, rangeMin.y, rangeMin.z);
                    points[1].position = new Vector3(0, rangeMin.y, rangeMin.z * arcMidModifier);
                    points[2].position = new Vector3(rangeMax.x, rangeMin.y, rangeMin.z);
                    break;
                case 1:
                    points[0].position = new Vector3(rangeMax.x, rangeMin.y, rangeMin.z);
                    points[1].position = new Vector3(rangeMax.x * arcMidModifier, rangeMin.y, 0);
                    points[2].position = new Vector3(rangeMax.x, rangeMin.y, rangeMax.z);
                    break;
                case 2:
                    points[0].position = new Vector3(rangeMax.x, rangeMin.y, rangeMax.z);
                    points[1].position = new Vector3(0, rangeMin.y, rangeMax.z * arcMidModifier);
                    points[2].position = new Vector3(rangeMin.x, rangeMin.y, rangeMax.z);
                    break;
                case 3:
                    points[0].position = new Vector3(rangeMin.x, rangeMin.y, rangeMax.z);
                    points[1].position = new Vector3(rangeMin.x * arcMidModifier, rangeMin.y, 0);
                    points[2].position = new Vector3(rangeMin.x, rangeMin.y, rangeMin.z);
                    break;
            }
        }
    }
    Vector3 GenerateRandomPosition()
    {
        float randomX = Random.Range(rangeMin.x, rangeMax.x);
        float randomY = Random.Range(rangeMin.y, rangeMax.y);
        float randomZ = Random.Range(rangeMin.z, rangeMax.z);
        int axis = Random.Range(0, 2);
        int minOrMax = Random.Range(0, 2);
        switch (axis)
        {
            case 0:
                {
                    if (minOrMax == 0) randomX = rangeMin.x;
                    else randomX = rangeMax.x;
                    break;
                }
            case 1:
                {
                    if (minOrMax == 0) randomZ = rangeMin.x;
                    else randomZ = rangeMax.x;
                    break;
                }
        }

        return new Vector3(randomX, randomY, randomZ);
    }
}
