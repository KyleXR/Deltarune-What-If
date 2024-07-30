using UnityEngine;
using PathCreation;
using PathCreation.Examples;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProceduralMeshGenerator : MonoBehaviour
{
    public float trackWidth = 1.0f;

    private void Start()
    {
        GenerateTrack();
    }

    private void GenerateTrack()
    {
        PathCreator pathCreator = GetComponent<PathCreator>();
        if (pathCreator == null) return;

        VertexPath path = pathCreator.path;

        Vector3[] vertices = new Vector3[path.NumPoints * 2];
        int[] triangles = new int[(path.NumPoints - 1) * 6];

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 point = path.GetPoint(i);
            Vector3 forward = path.GetDirection(i);
            Vector3 left = Vector3.Cross(forward, Vector3.up).normalized;

            vertices[i * 2] = point - left * trackWidth / 2f;
            vertices[i * 2 + 1] = point + left * trackWidth / 2f;

            if (i < path.NumPoints - 1)
            {
                int startIndex = i * 6;
                triangles[startIndex] = i * 2;
                triangles[startIndex + 1] = i * 2 + 1;
                triangles[startIndex + 2] = (i + 1) * 2;

                triangles[startIndex + 3] = (i + 1) * 2;
                triangles[startIndex + 4] = i * 2 + 1;
                triangles[startIndex + 5] = (i + 1) * 2 + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
