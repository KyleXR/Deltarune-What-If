using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
	[SerializeField] Rigidbody rb;
    [SerializeField] FirstPersonController controller;
    public float speedMultiplier = 0.1f;

	public void Update()
	{
		float currentSpeed = rb.velocity.magnitude;

		if (currentSpeed > 3)
		{
			animator.speed = currentSpeed * speedMultiplier;
		}
		else
		{
			animator.speed = 1f;
		}
	}
}
