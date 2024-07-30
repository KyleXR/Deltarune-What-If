using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class ProcedruarlTriggers : MonoBehaviour
{
    SplineComputer spline;
    SplineFollower cart;
    public List<Transform> newPoints;
    SplinePoint lastpoint;
    
    void Start()
    {
        lastpoint = new SplinePoint();
        lastpoint.position = (Vector3.forward);
        lastpoint.normal = (Vector3.up) * 10;
        lastpoint.size = 1.0f;
        lastpoint.color = Color.white;
        spline = GetComponent<SplineComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewTrackMesh()
    {
        int startIndex = spline.pointCount;

        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[5];

        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = (Vector3.forward * (i + 1) * 25) + lastpoint.position;
            points[i].normal = (Vector3.up * i) * 10;
            points[i].size = 1.0f;
            points[i].color = Color.white;
        }

        lastpoint = points[points.Length - 1];
        startIndex = spline.pointCount;
        spline.AppendPoints(points.Length);
        int test = 0;
        for (int i = startIndex; i < spline.pointCount; i++)
        {
            spline.SetPoint(i, points[test]);
            test++;
        }
        
        spline.Rebuild();
        
        int rand = Random.Range(0, 5);

        switch (rand) 
        {
            case 0:
                {
                    //splineComputer.AddNodeLink(node, 1);
                    break;
                }
            case 1:
                {

                    break;
                }
            case 2:
                {

                    break;
                }
            case 3:
                {

                    break;
                }
            case 5:
                {

                    break;
                }
            default:
                break;
        }
    }
}
