using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;


    [SerializeField] private float moveSpeed = 10f;

    private Vector3 moveDir;

    [SerializeField] private InputActionReference moveIAR;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        

    }

    private void Update()
    {
        moveDir = moveIAR.action.ReadValue<Vector2>();   
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, 0, moveDir.y * moveSpeed);

    }
}
