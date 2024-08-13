using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    [SerializeField] private float moveSpeed = 10f;

    private Vector3 moveDir;

    [SerializeField] private InputActionReference moveIAR;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {
        moveDir = moveIAR.action.ReadValue<Vector2>();

        Animation();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, 0, moveDir.y * moveSpeed);

    }

    void Animation()
    {
        /*if(rb.velocity.magnitude <= 0.2)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }*/


        if(moveDir.magnitude >= 0.1)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
