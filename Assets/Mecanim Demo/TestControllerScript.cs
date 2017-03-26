using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControllerScript : MonoBehaviour
{
    private Animator animator;

	private void Start ()
    {
        animator = GetComponent<Animator>();
	}
	
	private void Update ()
    {
        HandleInput();
	}

    private void HandleInput()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        animator.SetFloat("VerticalSpeed", vertical);
        animator.SetFloat("HorizontalSpeed", horizontal);
    }
}