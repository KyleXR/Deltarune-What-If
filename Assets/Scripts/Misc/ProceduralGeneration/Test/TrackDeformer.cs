using PathCreation;
using UnityEngine;

public class TrackDeformer : MonoBehaviour
{
    public GameObject trackPrefab; // Prefab of the track segment
    public PathCreator pathCreator;  // Your Bezier path class

    private void Start()
    {
        DeformTrack();
    }

    private void DeformTrack()
    {
        Mesh trackMesh = trackPrefab.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = trackMesh.vertices;
        Vector3[] deformedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            // Get the vertex position in world space
            Vector3 vertexPosition = trackPrefab.transform.TransformPoint(vertices[i]);

            // Find the closest point on the Bezier path to the vertex
            Vector3 closestPoint = pathCreator.path.GetClosestPointOnPath(vertexPosition);


            // Calculate the deformation offset
            Vector3 deformationOffset = closestPoint - vertexPosition;

            // Apply the deformation
            deformedVertices[i] = vertices[i] + deformationOffset;
        }

        // Update the mesh vertices
        trackMesh.vertices = deformedVertices;
        trackMesh.RecalculateNormals();
        trackMesh.RecalculateBounds();
    }
}
