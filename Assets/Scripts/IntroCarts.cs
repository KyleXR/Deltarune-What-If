using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCarts : MonoBehaviour
{
    [SerializeField] SplineFollower[] splineFollowers;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < splineFollowers.Length; i++) 
        {
            splineFollowers[i].followSpeed = 0;
        }
    }

    public void StartIntro()
    {
        for (int i = 0; i < splineFollowers.Length; i++)
        {
            splineFollowers[i].followSpeed = 20;
        }
    }
}
