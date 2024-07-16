using System.Collections.Generic;
using UnityEngine;

namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class ProceduralPathGenerator : MonoBehaviour 
    {

        private PathCreator creator;
        public bool closedLoop = true;
        public bool hasTurned = false;
        public float updateTimer = 0.0f;
        public GameObject sphere;
        public PathFollower cart;
        public float spherePosition = 0;
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

        public void GenerateCurvedPath()
        {
            //float x = 0f; // replace with your desired rotation value
            //float y = 0f; // replace with your desired rotation value
            //float z = 0f; // replace with your desired rotation value

            // Create the Quaternion for the desired rotation
            //Quaternion rotation = Quaternion.Euler(x, y, z);

            var sphereOne = Instantiate(sphere, new Vector3(5, 0, spherePosition), Quaternion.identity); //rotation);
            sphereOne.transform.rotation = Quaternion.LookRotation(new Vector3(0,90,0), new Vector3(0,0,0));
            waypoints.Add(sphereOne.transform);
            hasTurned = !hasTurned;
            GeneratePath();
            creator.TriggerPathUpdate();

        }

        public void GenerateWaypoint()
        {
            float offset = (hasTurned ? 10 : 0);
            
            if(!hasTurned) spherePosition += 34.5f;
            else spherePosition -= 34.5f;


            if (waypoints.Count > 4)
            {
                var lastPosition = waypoints[0];
                waypoints.Remove(lastPosition);
                Destroy(lastPosition.gameObject);
            }

            if (spherePosition == 1000.5f || spherePosition == -1000.5f)
            {
                GenerateCurvedPath();
            }
            else
            {
                var newSphere = Instantiate(sphere, new Vector3(offset, 0, spherePosition), Quaternion.identity);
                newSphere.transform.rotation = Quaternion.LookRotation(new Vector3(0, 90, 0), new Vector3(0, 0, 0));
                waypoints.Add(newSphere.transform);
            }
            GeneratePath();
            creator.TriggerPathUpdate();
        }
    }
}  