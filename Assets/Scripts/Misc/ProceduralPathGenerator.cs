using UnityEngine;

namespace PathCreation.Examples {
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class ProceduralPathGenerator : MonoBehaviour {

        public bool closedLoop = true;
        public Transform[] waypoints;
        public bool isManual = false;

        void Start () {
            if (!isManual) { GeneratePath(); }
        }

        public void GeneratePath()
        {
            if (waypoints.Length > 0)
            {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
                var oldBezier = GetComponent<PathCreator>().bezierPath;
                float globalNormals = oldBezier.GlobalNormalsAngle;
                GetComponent<PathCreator>().bezierPath = bezierPath;
                bezierPath.GlobalNormalsAngle = globalNormals;
            }
        }
    }
}   