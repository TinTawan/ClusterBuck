using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint rootJoint;

    [SerializeField] private Transform cam;

    [SerializeField] private float moveSpeed = 10f;
    //private Vector3 moveDir;
    private Vector2 moveInput;
    [SerializeField] private float rotateSmooth = 0.1f;

    [SerializeField] private InputActionReference moveIAR;



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

        Vector3 camForward = cam.right;
        Vector3 camRight = cam.forward;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRel = moveInput.x * camForward;
        Vector3 rightRel = moveInput.y * camRight;
        Vector3 camMoveDir = forwardRel + rightRel;


        Vector3 moveDir = new Vector3(camMoveDir.x, 0, camMoveDir.z);

        if (moveDir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            Quaternion rotation = Quaternion.Euler(0, lookRot.eulerAngles.y + 180, 0);

            //Quaternion test = Quaternion.Lerp(lookRot, Quaternion.Euler(0, lookRot.eulerAngles.y + 180, 0), rotateSmooth * Time.deltaTime);
            rootJoint.targetRotation = Quaternion.Inverse(rotation);

            rb.velocity = moveDir * moveSpeed;
        }
    }
 
    
}
