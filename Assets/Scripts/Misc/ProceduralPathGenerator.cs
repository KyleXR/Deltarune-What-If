using System.Collections;
using System.Collections.Generic;
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
            Queue<Transform> test = new Queue<Transform>(waypoints);
            
            if (waypoints.Length > 0)
            {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath(test, closedLoop, PathSpace.xyz);
                var oldBezier = GetComponent<PathCreator>().bezierPath;
                float globalNormals = oldBezier.GlobalNormalsAngle;
                GetComponent<PathCreator>().bezierPath = bezierPath;
                bezierPath.GlobalNormalsAngle = globalNormals;
            }
        }
    }
}   