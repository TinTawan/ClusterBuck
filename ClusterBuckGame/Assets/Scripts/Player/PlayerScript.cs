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
    private Vector2 moveInput;

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
        if (!IsOwner) return;
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
            //                              ==== ==== 
            //old code to rotate player in direction they walk (not using anymore)
            /*Quaternion lookRot = Quaternion.LookRotation(moveDir);
            Quaternion rotation = Quaternion.Euler(0, lookRot.eulerAngles.y + 180, 0);

            rootJoint.targetRotation = Quaternion.Inverse(rotation);*/
            //                              ==== ====



            //rotate the player so forward is the same as the camera's forward
            Quaternion rotation = Quaternion.Euler(0, cam.eulerAngles.y + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;
        }

    }
 
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}
