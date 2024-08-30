using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackPrefabSpawner : MonoBehaviour
{
    [SerializeField] TrackPrefab[] tracks;
    List<TrackPrefab> trackList = new List<TrackPrefab>();
    Transform lastNodeTransform;
    Vector3[] cartPositions = new Vector3[3];
    Vector3[,] followCartPositions = new Vector3[3,2];
    [SerializeField] SplineComputer[] trackSplines;
    [SerializeField] SplineFollower[] carts;
    [SerializeField] CartFollowerManager[] followManager;
    TrackPrefab newTrack;
    bool isFirstTrack = true;

    public bool canSpawn3rdTrack = true;
    public float currentDirection = 0;
    private float currentPadding = 0;
    public float time;
    public int maxPrefabs = 5;
    public int timesCalled = 0;
    public Scene newScene = new Scene();

    void Start()
    {
        newScene = SceneManager.GetSceneByName("SampleMapScene");
        lastNodeTransform = transform;
        StartCoroutine(InitializeTrack());
    }

    IEnumerator InitializeTrack()
    {
        while(newScene.IsValid())
        {
            yield return null;
        }

        for (int i = 0; i < maxPrefabs; i++)
        {
            SpawnTrackPrefab();
        }

        foreach (var cart in carts) cart.RebuildImmediate();

        foreach (var cart in followManager) cart.InitializeCarts();

        var followerAmount = followManager[0].additionalCartFollowers.Length;

        followCartPositions = new Vector3[followManager.Length, followerAmount];
    }

    public void SpawnTrackPrefab()
    {
        timesCalled++;
        //Debug.Log("I've been called, " + timesCalled + " times");


        
        // Save the world positions of the carts before updating the spline
        SaveCartPositions();

        if (isFirstTrack)
        {
            newTrack = Instantiate(tracks[0], lastNodeTransform.position, lastNodeTransform.rotation);
            isFirstTrack = false;
        }


        else
        {
            // Step 1: Randomly select a track prefab from the array
            int rand = Random.Range(0, tracks.Length);
            //Debug.Log("I am being called");

            // Step 2: Instantiate the selected track prefab at the last node's position and rotation
            newTrack = Instantiate(tracks[rand], lastNodeTransform.position, lastNodeTransform.rotation);
            trackSplines[1].ResetTriggers();
        }

       

        // Step 3: Rotate the new track based on the current direction
        newTrack.transform.rotation = Quaternion.Euler(0, currentDirection, 0);

        // Step 4: Calculate the offset position for the new track after applying the rotation
        float newOffset = newTrack.spawnOffset + currentPadding;
        Vector3 positionOffset = newTrack.transform.rotation * (Vector3.forward * newOffset);
        newTrack.transform.position = lastNodeTransform.position + positionOffset;

        // Step 5: Update the last node transform and add the new track to the list
        lastNodeTransform = newTrack.lastNodeTransform;
        trackList.Add(newTrack);

        // Step 6: Update the current direction and padding for the next track
        currentDirection += newTrack.endDirection;
        currentPadding = newTrack.spawnPadding;

        // Step 7: Spawn track splines if necessary
        for (int i = 0; i < trackSplines.Length; i++)
        {
            if (i == trackSplines.Length - 1 && !canSpawn3rdTrack) continue;

            SpawnTrackSpline(trackSplines[i], newTrack.GetTrackNodes(i + 1), i);

            if (trackList.Count > maxPrefabs)
            {
                RemoveTrackSpline(trackSplines[i], i);
            }
        }
        if (trackList.Count > maxPrefabs)
        {
            Destroy(trackList[0].gameObject);
            trackList.RemoveAt(0);
        }

        if (newTrack.name == "FRTR_Track(Clone)")
        {
            canSpawn3rdTrack = false;
        }

        if (newTrack.name == "FRTL_Track(Clone)")
        {
            canSpawn3rdTrack = true;
        }
        var middleNodes = newTrack.GetTrackNodes(2);
        if (trackList.Count > 2) middleNodes = trackList[2].GetTrackNodes(2);
        var triggerNode = trackSplines[1].Project(middleNodes[middleNodes.Length / 2].position).percent;
        trackSplines[1].triggerGroups[0].triggers[0].position = triggerNode;
        //Debug.Log(triggerNode.ToString());

        // Update the positions of the carts on the spline
        UpdateCartPositions();

        
    }

    public void SpawnTrackSpline(SplineComputer spline, Transform[] nodeTransforms, int id)
    {
        // Create a new array of spline points
        SplinePoint[] points = new SplinePoint[nodeTransforms.Length];

        // Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint
            {
                position = nodeTransforms[i].position,
                normal = Vector3.up, // Assuming up direction for normal, adjust if needed
                size = 1.0f,
                color = Color.white
            };
        }

        // Append the new points to the spline
        int startIndex = spline.pointCount;
        spline.AppendPoints(points.Length);
        for (int i = 0; i < points.Length; i++)
        {
            spline.SetPoint(startIndex + i, points[i]);
        }

        spline.RebuildImmediate();
    }

    public void RemoveTrackSpline(SplineComputer spline, int id)
    {
        if (trackList != null)
        {
            var nodes = trackList[0].GetTrackNodes(id + 1).Length;
            SplinePoint[] points = spline.GetPoints();
            SplinePoint[] newPoints = new SplinePoint[points.Length - nodes];
            for (int i = nodes; i < points.Length; i++)
            {
                newPoints[i - nodes] = points[i];
            }
            spline.SetPoints(newPoints);
            spline.RebuildImmediate();
        }
    }

    public void SaveCartPositions()
    {
        // Save the current world positions of each cart
        for (int i = 0; i < cartPositions.Length; i++)
        {
            cartPositions[i] = carts[i].transform.position;
        }

        for (int i = 0; i < followCartPositions.GetLength(0); i++)
        {
            for(int j = 0; j < followCartPositions.GetLength(1); j++) 
            {
                followCartPositions[i,j] = followManager[i].additionalCartFollowers[j].transform.position;
            }
        }
    }

    public void UpdateCartPositions()
    {
        // Update the position of each cart on the spline based on the saved world positions
        for (int i = 0; i < carts.Length; i++)
        {
            var sample = trackSplines[i].Project(cartPositions[i]);
            carts[i].SetPercent(sample.percent);
            carts[i].RebuildImmediate();
        }

        for (int i = 0; i < followManager.Length; i++)
        {
            for(int j = 0; j < followManager[i].additionalCartFollowers.Length; j++)
            {
                var sample = trackSplines[i].Project(followCartPositions[i,j]);
                followManager[i].additionalCartFollowers[j].SetPercent(sample.percent);
                followManager[i].additionalCartFollowers[j].RebuildImmediate();
            }
            
        }
    }
}
