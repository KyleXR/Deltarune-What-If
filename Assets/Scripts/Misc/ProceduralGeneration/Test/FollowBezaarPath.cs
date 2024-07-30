using UnityEngine;
using PathCreation; // Assuming you are using the PathCreator package

public class FollowBezierPath : MonoBehaviour
{
    public GameObject model; // Reference to the model in the scene
    public Vector3[] waypoints;
    public bool closedLoop = false;

    private BezierPath bezierPath;
    private VertexPath vertexPath;

    void Start()
    {
        // Ensure the model reference is set
        if (model == null)
        {
            Debug.LogError("Model reference is not set.");
            return;
        }

        // Create the Bezier path
        CreateBezierPath();

        // Make the model follow the path
        StartCoroutine(FollowPath());
    }

    void CreateBezierPath()
    {
        bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
        PathCreator pathCreator = GetComponent<PathCreator>();
        BezierPath oldBezier = pathCreator.bezierPath;
        float globalNormals = oldBezier.GlobalNormalsAngle;
        pathCreator.bezierPath = bezierPath;
        bezierPath.GlobalNormalsAngle = globalNormals;
        vertexPath = new VertexPath(bezierPath, transform);
    }

    System.Collections.IEnumerator FollowPath()
    {
        float distanceTravelled = 0f;
        float speed = 5f; // Adjust the speed as necessary

        while (true)
        {
            distanceTravelled += speed * Time.deltaTime;
            Vector3 nextPosition = vertexPath.GetPointAtDistance(distanceTravelled);
            model.transform.position = nextPosition;
            yield return null;
        }
    }
}
