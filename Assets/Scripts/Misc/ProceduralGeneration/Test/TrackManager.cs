using PathCreation;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject trackPrefab;
    public PathCreator pathCreator;
    public int numberOfSegments = 10;

    private void Start()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            float t = (float)i / (numberOfSegments - 1);
            Vector3 position = pathCreator.path.GetPointAtTime(t);
            Quaternion rotation = Quaternion.LookRotation(pathCreator.path.GetDirection(t));

            GameObject trackSegment = Instantiate(trackPrefab, position, rotation);
            trackSegment.AddComponent<TrackDeformer>().pathCreator = pathCreator;
        }
    }
}