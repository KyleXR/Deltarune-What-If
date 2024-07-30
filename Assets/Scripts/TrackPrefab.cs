using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPrefab : MonoBehaviour
{
    [SerializeField] Transform[] trackOneNodes;
    [SerializeField] Transform[] trackTwoNodes;
    [SerializeField] Transform[] trackThreeNodes;

    public float endDirection = 0;
    public Transform lastNodeTransform;
    public Transform firstNodeTransform;
    public float spawnOffset = 25;
    public float distance = 50;
    public float spawnPadding = 50;
    
    void Awake()
    {
        distance = Vector3.Distance(trackTwoNodes[0].position, trackTwoNodes[trackTwoNodes.Length - 1].position);
    }

    public Transform[] GetTrackNodes(int trackID)
    {
        switch(trackID)
        {
            case 1:
                return trackOneNodes;
            case 2:
                return trackTwoNodes;
            case 3:
                return trackThreeNodes;
            default:
                return null;
        }
    }
}
