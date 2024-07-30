using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace PathCreation.Examples
{
    public class ProceduralSplineGenerator : MonoBehaviour
    {

        public List<Vector3> waypoints;
        private PathCreator creator;
        public PathFollower cart;
        public bool closedLoop = false;
        public float waypointDistance = 50.0f;
        public float maxDistance = 1000.0f;

        private BezierPath bezierPath;

        private void Start()
        {
            creator = GetComponent<PathCreator>();
            GenerateInitialPath();
        }

        private void Update()
        {
            float percetageTravelled = cart.distanceTravelled / creator.path.length;
            if (percetageTravelled >= 0.8f)
            {
                
                //GeneratePath();
            }
        }

        private void GenerateInitialPath()
        {
            if (waypoints.Count < 2)
            {
                Debug.LogError("At least two waypoints are needed to generate a path.");
                return;
            }

            GeneratePath();
        }

        public void AddWaypoint(Vector3 newWaypoint)
        {
            waypoints.Add(newWaypoint);
            GeneratePath();
        }

        private void GeneratePath()
        {
            bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
            var oldBezier = GetComponent<PathCreator>().bezierPath;
            float globalNormals = oldBezier.GlobalNormalsAngle;
            GetComponent<PathCreator>().bezierPath = bezierPath;
            bezierPath.GlobalNormalsAngle = globalNormals;
        }

        public Vector3 GetNextWaypoint(int index)
        {
            if (index >= waypoints.Count)
            {
                return Vector3.zero;
            }
            return waypoints[index];
        }

        public int GetWaypointCount()
        {
            return waypoints.Count;
        }
    }
}
