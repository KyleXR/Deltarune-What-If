using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCarts : MonoBehaviour
{
    [SerializeField] SplineFollower[] splineFollowers;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform targetCart;
    private bool rotateToCart = true;
    public float rotationSpeed = 5.0f;
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
        //StartCoroutine(TurnCamera());
        for (int i = 0; i < splineFollowers.Length; i++)
        {
            splineFollowers[i].followSpeed = 20;
        }
    }
}
