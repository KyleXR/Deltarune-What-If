using System.Collections.Generic;
using UnityEngine;

namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class ProceduralPathGenerator : MonoBehaviour {

        private PathCreator creator;
        public bool closedLoop = true;
        public float updateTimer = 0.0f;
        public GameObject sphere;
        public PathFollower cart;
        public int spherePosition = 0;
        public List<Transform> waypoints;

        void Start () 
        {
            creator = GetComponent<PathCreator>();
            GenerateWaypoint();
            //GeneratePath();
        }


        void Update () 
        {
            float percetageTravelled = cart.distanceTravelled / creator.path.length;
            if(percetageTravelled > 0.8f)
            {
                GenerateWaypoint();
            }
        }
        public void GeneratePath()
        {
            if (waypoints.Count > 0)
            {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
                var oldBezier = GetComponent<PathCreator>().bezierPath;
                float globalNormals = oldBezier.GlobalNormalsAngle;
                GetComponent<PathCreator>().bezierPath = bezierPath;
                bezierPath.GlobalNormalsAngle = globalNormals;
            }
        }

        public void GenerateWaypoint()
        {
            var newSphere = Instantiate(sphere, new Vector3(0, 0, spherePosition), Quaternion.identity);
            waypoints.Add(newSphere.transform);
            spherePosition += 5;
            updateTimer = 0.0f;


            if (waypoints.Count > 4)
            {
                var lastPosition = waypoints[0];
                waypoints.Remove(lastPosition);
                Destroy(lastPosition.gameObject);
            }

            GeneratePath();
            creator.TriggerPathUpdate();
        }
    }
}  