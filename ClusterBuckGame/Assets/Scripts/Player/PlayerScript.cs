using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Windows;
using Unity.Netcode.Components;
using Unity.VisualScripting;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint rootJoint;
    private CinemachineInputProvider cinInputProvider;

    private Transform cam;


    [SerializeField] private float moveSpeed = 10f;
    private Vector2 moveInput;

    private PlayerInput playerInput;
    private PlayerControlsActions playerControlsActions;
    [SerializeField] private InputActionReference moveIAR;

    [SerializeField] private CinemachineFreeLook freeLookCam;
    [SerializeField] private AudioListener listener;

    //Quaternion rotation = Quaternion.identity;


    [SerializeField] private float spawnPosRange = 3f;

    private NetworkVariable<float> networkedYRotation = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkXSens = new NetworkVariable<float>(2500, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkYSens = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private bool isChargingAttack;
    int x = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //assign components
        rb = GetComponent<Rigidbody>();
        rootJoint = GetComponent<ConfigurableJoint>();
        cinInputProvider = FindObjectOfType<CinemachineInputProvider>();
        freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        playerInput = GetComponent<PlayerInput>();

        if (IsClient && IsOwner)
        {
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

        //setup player controls actions
        playerControlsActions = new PlayerControlsActions();
        playerControlsActions.General.Enable();

        playerControlsActions.General.Attack.performed += HoldAttack;
        playerControlsActions.General.Attack.canceled += ReleaseAttack;

        //set randomised spawn position around the origin
        transform.position = new Vector3(Random.Range(spawnPosRange, -spawnPosRange), 0, (Random.Range(spawnPosRange, -spawnPosRange)));

        //set the freelookcam sensitivity to the networked variable
        freeLookCam.m_XAxis.m_MaxSpeed = networkXSens.Value;
        freeLookCam.m_YAxis.m_MaxSpeed = networkYSens.Value;
    }

    private void HoldAttack(InputAction.CallbackContext context)
    {
        isChargingAttack = true;
    }

    private void ReleaseAttack(InputAction.CallbackContext context)
    {
        isChargingAttack = false;
        x = 0;
        Debug.Log("Release Attack");
    }

    void TestCharging()
    {
        if(isChargingAttack)
        {
            
            if(x < 50)
            {
                x += 1;
                Debug.Log(x);
            }
            
            if (x == 50)
            {
                Debug.Log("Max charge");
            }
            
            
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        //read the vector 2 from the General action controls
        moveInput = playerControlsActions.General.Move.ReadValue<Vector2>();

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

        PlayerMovementServAuth(moveInput);
        TestCharging();
    }

    void PlayerMovementServAuth(Vector2 moveInput)
    {
        PlayerMovementServerRpc(moveInput, cam.transform.rotation);
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
            Quaternion rotation = Quaternion.Euler(0, networkedYRotation.Value + 180, 0);
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

    public bool GetIsChargingAttack()
    {
        return isChargingAttack;
    }








    //keeping in case
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

            Quaternion rotation = Quaternion.Euler(0, networkedYRotation.Value + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;
        }

    }
}
