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

    private void Start()
    {
        points = pathGenerator.waypoints;
        TargetPlayer();
        pathGenerator.waypoints = points;
        pathGenerator.GeneratePath();
    }
    public void TargetPlayer()
    {
        points[0].position = GenerateRandomPosition();
        //points[1].position = GenerateRandomPosition();
        points[1].position = playerTarget.position;
        points[2].position = GenerateRandomPosition();
    }

    Vector3 GenerateRandomPosition()
    {
        float randomX = Random.Range(rangeMin.x, rangeMax.x);
        float randomY = Random.Range(rangeMin.y, rangeMax.y);
        float randomZ = Random.Range(rangeMin.z, rangeMax.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
