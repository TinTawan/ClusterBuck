using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint rootJoint;


    [SerializeField] private float moveSpeed = 10f;
    //private Vector3 moveDir;
    private Vector2 moveInput;
    [SerializeField] private float rotateSmooth = 0.1f;
    float rotateSmoothVel;

    [SerializeField] private InputActionReference moveIAR;


    private Quaternion startRootRotation;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rootJoint = GetComponent<ConfigurableJoint>();

    }

    private void Update()
    {
        moveInput = moveIAR.action.ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        
        PlayerMovement();

    }

    void PlayerMovement()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Quaternion newRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y + 180, 0);

            rootJoint.targetRotation = Quaternion.Inverse(newRotation);

            rb.velocity = moveDir * moveSpeed;
        }
    }
 
    
}
