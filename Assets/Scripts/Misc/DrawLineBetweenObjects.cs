using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineBetweenObjects : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;

    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        // Set the number of line segments (2 for a simple line)
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (object1 != null && object2 != null && lineRenderer.positionCount >= 2)
        {
            // Set the positions of the line
            lineRenderer.SetPosition(0, object1.transform.position);
            lineRenderer.SetPosition(1, object2.transform.position);
        }
    }
}
