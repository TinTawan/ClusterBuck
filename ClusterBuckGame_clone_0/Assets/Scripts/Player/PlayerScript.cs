using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using Cinemachine;
using UnityEngine.Windows;
using Unity.Netcode.Components;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint rootJoint;
    private CinemachineInputProvider cinInputProvider;

    private Transform cam;


    [SerializeField] private float moveSpeed = 10f;
    private Vector2 moveInput;

    [SerializeField] private InputActionReference moveIAR;

    [SerializeField] private CinemachineFreeLook freeLookCam;
    [SerializeField] private AudioListener listener;

    Quaternion rotation = Quaternion.identity;
    Vector3 moveDir = new Vector3(), camMoveDir;

    [SerializeField] private float spawnPosRange = 3f;

    private NetworkVariable<float> networkedYRotation = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();
        rootJoint = GetComponent<ConfigurableJoint>();
        //cinInputProvider = GetComponentInChildren<CinemachineInputProvider>();
        cinInputProvider = FindObjectOfType<CinemachineInputProvider>();
        freeLookCam = FindObjectOfType<CinemachineFreeLook>();

        if (IsClient && IsOwner)
        {
            //cam = GetComponentInChildren<Camera>().transform;
            cam = FindObjectOfType<Camera>().transform;
            cam.gameObject.SetActive(true);

            
            cinInputProvider.AutoEnableInputs = true;
            cinInputProvider.PlayerIndex = (int)NetworkManager.LocalClientId;

            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;

        }
        else if (IsOwner)
        {
            listener.enabled = true;
            freeLookCam.Priority = 2;
            
        }
        else
        {
            freeLookCam.Priority = 0;
        }

        transform.position = new Vector3(Random.Range(spawnPosRange, -spawnPosRange), 0, (Random.Range(spawnPosRange, -spawnPosRange)));
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        moveInput = moveIAR.action.ReadValue<Vector2>();

        networkedYRotation.Value = cam.eulerAngles.y;

        /*//check if player or server
        if (IsServer && IsLocalPlayer)
        {
            //move if server
            PlayerMovement(moveInput);
            //Debug.Log("Host: " + NetworkManager.LocalClientId);
        }
        else if(IsClient && IsLocalPlayer)
        {
            //request move if player
            MoveServerRpc(moveInput);
            Debug.Log("MoveDir: " + (new Vector3(((moveInput.x * cam.right) + moveInput.y * cam.forward).x, 0, ((moveInput.x * cam.right) + moveInput.y * cam.forward).z))*moveSpeed);
        }*/

        //PlayerMovement(moveInput);
        //serverauth test
        PlayerMovementServAuth(moveInput);

    }

    void PlayerMovementServAuth(Vector2 moveInput)
    {
        Debug.Log("cam rotation: " + cam.eulerAngles);
        PlayerMovementServerRpc(moveInput, cam.transform.rotation);
    }

    void PlayerMovement(Vector2 input)
    {

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRel = input.x * camRight;
        Vector3 rightRel = input.y * camForward;
        Vector3 camMoveDir = forwardRel + rightRel;

        Vector3 moveDir = new Vector3(camMoveDir.x, 0, camMoveDir.z);

        Debug.Log(moveDir);
        if (moveDir != Vector3.zero)
        {
            //Debug.Log(moveDir);
            //                              ==== ==== 
            //old code to rotate player in direction they walk (not using anymore)
            /*Quaternion lookRot = Quaternion.LookRotation(moveDir);
            Quaternion rotation = Quaternion.Euler(0, lookRot.eulerAngles.y + 180, 0);

            rootJoint.targetRotation = Quaternion.Inverse(rotation);*/
            //                              ==== ====



            /*//check if player or server
            if (IsServer && IsLocalPlayer)
            {
                //rotate if server
                RotateRootBone(cam.eulerAngles.y);
                Quaternion invert = Quaternion.Inverse(rotation);
                rootJoint.targetRotation = invert;
            }
            else if (IsClient && IsLocalPlayer)
            {
                //request rotate if player
                RotateRootBoneServerRpc(cam.eulerAngles.y);
                Quaternion invert = Quaternion.Inverse(rotation);
                rootJoint.targetRotation = invert;
            }*/

            //RotateRootBoneServerRpc(networkedYRotation.Value);

            rotation = Quaternion.Euler(0, networkedYRotation.Value + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;
        }

    }

    [ServerRpc]
    void PlayerMovementServerRpc(Vector2 moveInput, Quaternion camInput)
    {
        Vector3 camForward = camInput * Vector3.forward;
        Vector3 camRight = camInput * Vector3.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRel = moveInput.x * camRight;
        Vector3 rightRel = moveInput.y * camForward;
        Vector3 camMoveDir = forwardRel + rightRel;

        Vector3 moveDir = new Vector3(camMoveDir.x, 0, camMoveDir.z);

        if (moveDir != Vector3.zero)
        {
            rotation = Quaternion.Euler(0, networkedYRotation.Value + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;
        }
    }

    /*void RotateRootBone(float input) 
    {
        //rotate the player so forward is the same as the camera's forward
        rotation = Quaternion.Euler(0, input + 180, 0);
        Quaternion invert = Quaternion.Inverse(rotation);
        rootJoint.targetRotation = invert;

        
    }

    Vector3 CamLogic(Vector2 input)
    {
        Vector3 camForward = cam.right;
        Vector3 camRight = cam.forward;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRel = input.x * camForward;
        Vector3 rightRel = input.y * camRight;
        camMoveDir = forwardRel + rightRel;
        return camMoveDir;
    }*/

    /*[ServerRpc]
    private void MoveServerRpc(Vector2 input)
    {
        PlayerMovement(input);
    }

    [ServerRpc]
    private void RotateRootBoneServerRpc(float input)
    {
        Debug.Log("Client: " + NetworkManager.LocalClientId);
        RotateRootBone(input);
    }*/

    /*[ServerRpc]
    private void CamLogicServerRpc(Vector2 input)
    {
        camMoveDir = CamLogic(input);
    }*/



    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}
