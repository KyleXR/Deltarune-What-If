using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAnimation : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)) 
        {
            animator.SetTrigger("Attack");
        }
    }
}
