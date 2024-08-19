using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Windows;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor.Build.Content;

public class PlayerScript : NetworkBehaviour
{
    private Rigidbody rb;
    private ConfigurableJoint rootJoint;
    private CinemachineInputProvider cinInputProvider;

    private Transform cam;


    private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 3.5f, walkSpeed = 2f;

    private Vector2 moveInput;
    private Vector3 moveDir;

    private PlayerInput playerInput;
    private PlayerControlsActions playerControlsActions;

    [SerializeField] private CinemachineFreeLook freeLookCam;
    [SerializeField] private AudioListener listener;


    [SerializeField] private float spawnPosRange = 3f;

    private NetworkVariable<float> network_YRotation = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkX_Sens = new NetworkVariable<float>(2500, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> network_YSens = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> network_ChargeLevel = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private NetworkVariable<Vector2> network_MoveDirection = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> network_AttackDirection = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<float> network_MoveSpeed = new NetworkVariable<float>(2f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private bool isChargingAttack;
    float maxCharge = 50;

    //private GameObject gameUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //assign components
        rb = GetComponent<Rigidbody>();
        rootJoint = GetComponent<ConfigurableJoint>();
        cinInputProvider = FindObjectOfType<CinemachineInputProvider>();
        freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        playerInput = GetComponent<PlayerInput>();
        listener = FindObjectOfType<Camera>().GetComponent<AudioListener>();

        playerInput.camera = FindObjectOfType<Camera>();

        //gameUI = FindObjectOfType<Canvas>().transform.Find("GameUI").gameObject;

        if (IsClient && IsOwner)
        {
            cam = FindObjectOfType<Camera>().transform;
            cam.gameObject.SetActive(true);

            
            cinInputProvider.AutoEnableInputs = true;
            cinInputProvider.PlayerIndex = (int)NetworkManager.LocalClientId;

            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;

            //gameUI.SetActive(true);

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

        //cause callbacks when holding and then releasing the attack button
        playerControlsActions.General.Attack.performed += HoldAttack;
        playerControlsActions.General.Attack.canceled += ReleaseAttack;

        //set randomised spawn position around the origin
        transform.position = new Vector3(Random.Range(spawnPosRange, -spawnPosRange), 0, (Random.Range(spawnPosRange, -spawnPosRange)));

        //set the freelookcam sensitivity to the networked variable
        freeLookCam.m_XAxis.m_MaxSpeed = networkX_Sens.Value;
        freeLookCam.m_YAxis.m_MaxSpeed = network_YSens.Value;

        moveSpeed = walkSpeed;
        //networkRunSpeed.Value = runSpeed;
        network_MoveSpeed.Value = moveSpeed;

    }

    private void HoldAttack(InputAction.CallbackContext context)
    {
        isChargingAttack = context.ReadValueAsButton();

        SetMoveSpeed_ServerRpc(runSpeed);
    }

    private void ReleaseAttack(InputAction.CallbackContext context)
    {
        isChargingAttack = false;

        SetMoveSpeed_ServerRpc(walkSpeed);


        //reset network var for charge level to 0
        IncrementChargeLevel_ServerRpc(-network_ChargeLevel.Value);
        Debug.Log("Release Attack");

        
    }


    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        //read the vector 2 from the General action controls
        moveInput = playerControlsActions.General.Move.ReadValue<Vector2>();

        network_YRotation.Value = cam.eulerAngles.y;

        //check if player or server
        if (IsServer && IsLocalPlayer)
        {
            network_MoveDirection.Value = moveInput;
            network_AttackDirection.Value = -rootJoint.transform.forward;

        }
        else if (IsClient && IsLocalPlayer)
        {

            SetAttackDirection_ServerRpc(-rootJoint.transform.forward);
            SetMoveDir_ServerRpc(moveInput);
        }

        PlayerMovementServAuth();
        PlayerAttackingServAuth();

       
    }


    void PlayerMovementServAuth()
    {
        PlayerMovement_ServerRpc(network_MoveDirection.Value, cam.transform.rotation);
    }

    [ServerRpc]
    void PlayerMovement_ServerRpc(Vector2 moveInput, Quaternion camInput)
    {
        Vector3 camForward = camInput * Vector3.forward;
        Vector3 camRight = camInput * Vector3.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRel = moveInput.x * camRight;
        Vector3 rightRel = moveInput.y * camForward;
        Vector3 camMoveDir = forwardRel + rightRel;

        moveDir = new Vector3(camMoveDir.x, 0, camMoveDir.z);

        if (moveDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.Euler(0, network_YRotation.Value + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;

        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetMoveSpeed_ServerRpc(float speed)
    {
        network_MoveSpeed.Value = speed;
    }
    [ServerRpc(RequireOwnership = false)]
    void SetMoveDir_ServerRpc(Vector2 inVect)
    {
        network_MoveDirection.Value = inVect;
    }


    private void PlayerAttackingServAuth()
    {
        PlayerAttacking_ServerRpc(network_AttackDirection.Value, isChargingAttack);

    }

    [ServerRpc]
    private void PlayerAttacking_ServerRpc(Vector3 inputMoveDir, bool isAttacking)
    {
        if(isAttacking)
        {
            //charging up
            if (network_ChargeLevel.Value < maxCharge)
            {
                IncrementChargeLevel_ServerRpc(1);
                Debug.Log(network_ChargeLevel.Value);
            }

            //reached full charge but still holding
            if (network_ChargeLevel.Value == maxCharge)
            {
                Debug.Log("Max charge");
                
            }


            rb.velocity = inputMoveDir * network_MoveSpeed.Value;
        }
        

    }

    [ServerRpc(RequireOwnership = false)]
    void SetAttackDirection_ServerRpc(Vector3 inVect2)
    {
        network_AttackDirection.Value = inVect2;
    }

    [ServerRpc(RequireOwnership = false)]
    void IncrementChargeLevel_ServerRpc(float incrementVal)
    {
        network_ChargeLevel.Value += incrementVal;
    }


    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool GetIsChargingAttack()
    {
        return isChargingAttack;
    }

    public float GetChargeLevel()
    {
        return network_ChargeLevel.Value;
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

            Quaternion rotation = Quaternion.Euler(0, network_YRotation.Value + 180, 0);
            Quaternion invert = Quaternion.Inverse(rotation);
            rootJoint.targetRotation = invert;


            //move the player with given move speed
            rb.velocity = moveDir * moveSpeed;
        }

    }
}
