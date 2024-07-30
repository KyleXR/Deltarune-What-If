using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

public class TrackPrefabSpawner : MonoBehaviour
{
    [SerializeField] TrackPrefab[] tracks;
    List<TrackPrefab> trackList = new List<TrackPrefab>();
    Transform lastNodeTransform;
    List<Vector3> cartPostions = new List<Vector3>();
    [SerializeField] SplineComputer[] trackSplines;
    [SerializeField] List<Dreamteck.Splines.SplineFollower> carts;
    
    public bool canSpawn3rdTrack = true;
    public float currentDirection = 0;
    private float currentPadding = 0;
    public float time;

    
    void Start()
    {
        lastNodeTransform = transform;
        SpawnTrackPrefab();
        
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;
        if (time >= 8)
        {
            time = 0;
            SpawnTrackPrefab();

        }
    }

    public void SpawnTrackPrefab()
    {
        // Step 1: Randomly select a track prefab from the array
        int rand = Random.Range(0, tracks.Length);

        // Step 2: Instantiate the selected track prefab at the last node's position and rotation
        var newTrack = Instantiate(tracks[rand], lastNodeTransform.position, lastNodeTransform.rotation);

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
            
            SpawnTrackSpline(trackSplines[i], newTrack.GetTrackNodes(i + 1));

        }

        if (newTrack.name == "FRTR_Track(Clone)")
        {
            canSpawn3rdTrack = false;
        }

        if (newTrack.name == "FRTL_Track(Clone)")
        {
            canSpawn3rdTrack = true;
        }

        
        
    }



    public void SpawnTrackSpline(SplineComputer spline, Transform[] nodeTransforms)
    {
        //SetCartPostions();

        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[nodeTransforms.Length];

        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
 
            points[i] = new SplinePoint();
            points[i].position = nodeTransforms[i].position;
            points[i].normal = (Vector3.up * i) * 10;
            points[i].size = 1.0f;
            points[i].color = Color.white;
        }

        int startIndex = spline.pointCount;
        spline.AppendPoints(points.Length);
        int j = 0;

        for (int i = startIndex; i < spline.pointCount; i++)
        {
            spline.SetPoint(i, points[j]);
            j++;
        }
        spline.Rebuild();

    }

    //public void SetCartPostions()
    //{
    //    for(int i = 0; i < cartPostions.Count; i++) 
    //    {
    //        cartPostions[i] = carts[i].transform.position;
    //    }
    //}

    //public void UpdateCartPositions()
    //{
    //    for( int i = 0; i < carts.Count; i++) 
    //    {
    //        var newDistance = trackSplines[i].Project(cartPostions[i]);
    //        carts[i].SetPercent()
    //    }
    //}
}
