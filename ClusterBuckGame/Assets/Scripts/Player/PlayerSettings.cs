using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;


public class PlayerSettings : NetworkBehaviour
{
    private Canvas canvas;
    private Camera cam;
    private bool UiSet = false;


    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<FixedString32Bytes> network_PlayerName = new NetworkVariable<FixedString32Bytes>("Player 0",
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private SkinnedMeshRenderer playerMeshRend;

    [SerializeField] private List<Color> playerColorList = new List<Color>();


    private void Awake()
    {
        playerMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //playerName.Value = "Player " + OwnerClientId.ToString();
            network_PlayerName.Value = $"Player {OwnerClientId + 1}";

        }

        canvas = GetComponentInChildren<Canvas>();
        cam = FindObjectOfType<Camera>();

        canvas.worldCamera = cam;


        playerMeshRend.material.color = playerColorList[(int)OwnerClientId];
    }

    private void Update()
    {
        if (!UiSet && !network_PlayerName.Value.IsEmpty)
        {
            SetOverlay();
        }

        canvas.transform.LookAt(canvas.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    private void SetOverlay()
    {
        TextMeshProUGUI localPlayerUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerUI.text = network_PlayerName.Value.ToString();

        UiSet = true;
    }

    public List<Color> GetPlayerColourList()
    {
        return playerColorList;
    }
    public void SetPlayerColourInList(int playerID, Color inColour)
    {
        playerColorList[playerID] = inColour;
    }

}
