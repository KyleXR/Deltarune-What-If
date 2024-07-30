using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class ProceduralPathGenerator : MonoBehaviour 
    {

        public bool closedLoop = true;
        public bool hasTurned = false;
        public GameObject track;
        private PathCreator creator;
        public PathFollower cart;
        public float spherePosition = 0;
        public List<Transform> waypoints;

        void Start ()
        {
            creator = GetComponent<PathCreator>();
            GenerateWaypoint();
            GeneratePath();
        }


        void Update()
        {
            float percetageTravelled = cart.distanceTravelled / creator.path.length;
            if (percetageTravelled > 0.6f)
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

            var sphereOne = Instantiate(track, new Vector3(5, 0, spherePosition), Quaternion.identity); //rotation);
            sphereOne.transform.rotation = Quaternion.LookRotation(new Vector3(0, 90, 0), new Vector3(0, 0, 0));
            waypoints.Add(sphereOne.transform);
            hasTurned = !hasTurned;
            BendMeshToPath(sphereOne);
            GeneratePath();
            creator.TriggerPathUpdate();

        }

        public void GenerateWaypoint()
        {
            float offset = (hasTurned ? 10 : 0);

            if (!hasTurned) spherePosition += 34.5f;
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
                var newSphere = Instantiate(track, new Vector3(offset, 0, spherePosition), Quaternion.identity);
                newSphere.transform.rotation = Quaternion.LookRotation(new Vector3(0, 90, 0), new Vector3(0, 0, 0));
                waypoints.Add(newSphere.transform);
                BendMeshToPath(newSphere);
            }
            GeneratePath();
            creator.TriggerPathUpdate();
        }

        private void BendMeshToPath(GameObject sphere)
        {
            if (sphere == null || creator.path == null)
                return;

            Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                float t = (float)i / (vertices.Length - 1);
                Vector3 pointOnPath = creator.path.GetPointAtTime(t, EndOfPathInstruction.Stop);
                Vector3 direction = creator.path.GetDirection(t, EndOfPathInstruction.Stop);

                Quaternion rotation = Quaternion.LookRotation(direction);
                newVertices[i] = pointOnPath + rotation * vertices[i];
            }

            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
        }
    }
}  