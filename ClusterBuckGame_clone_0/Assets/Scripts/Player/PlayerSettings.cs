using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using Unity.VisualScripting;


public class PlayerSettings : NetworkBehaviour
{
    private Canvas canvas;
    private Camera cam;
    private bool UiSet = false;


    [SerializeField] private TextMeshProUGUI playerNameText;
    //private NetworkVariable<FixedString32Bytes> network_PlayerName = new NetworkVariable<FixedString32Bytes>("Player 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private SkinnedMeshRenderer playerMeshRend;

    //[SerializeField] private List<Color> playerColorList = new List<Color>();

    //private PlayerData playerData;
    //private string playerName;

    private NetworkVariable<PlayerData> network_PlayerData = new NetworkVariable<PlayerData>(new PlayerData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        playerMeshRend = GetComponentInChildren<SkinnedMeshRenderer>();

        
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //playerName.Value = "Player " + OwnerClientId.ToString();
            //network_PlayerName.Value = $"Player {OwnerClientId + 1}";


            //playerData = GetBuckedMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

            network_PlayerData.Value = GetBuckedMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

        }

        canvas = GetComponentInChildren<Canvas>();
        cam = FindObjectOfType<Camera>();

        canvas.worldCamera = cam;


        //playerMeshRend.material.color = playerColorList[(int)OwnerClientId];

        //PlayerData playerData = GetBuckedMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        

        //playerName = playerData.playerName.ToString();
        playerMeshRend.material.color = network_PlayerData.Value.playerColour;



    }

    private void Update()
    {
        if (!UiSet && !network_PlayerData.Value.playerName.IsEmpty)
        {
            SetOverlay();
        }

        canvas.transform.LookAt(canvas.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    private void SetOverlay()
    {
        TextMeshProUGUI localPlayerUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //localPlayerUI.text = network_PlayerName.Value.ToString();
        localPlayerUI.text = network_PlayerData.Value.playerName.ToString();

        
        UiSet = true;
    }

    /*public List<Color> GetPlayerColourList()
    {
        return playerColorList;
    }
    public void SetPlayerColourInList(int playerID, Color inColour)
    {
        playerColorList[playerID] = inColour;
    }*/

}
