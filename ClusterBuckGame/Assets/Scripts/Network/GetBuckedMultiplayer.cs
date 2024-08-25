using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.InputSystem.iOS;
using Unity.Collections;

public class GetBuckedMultiplayer : NetworkBehaviour
{
    public static GetBuckedMultiplayer Instance { get; private set; }

    private int maxPlayerCount = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

    private NetworkList<PlayerData> network_PlayerDataList;

    private List<Color> playerColourList;

    private void Awake()
    {
        /*if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(Instance);*/

        Instance = this;
        DontDestroyOnLoad(gameObject);

        network_PlayerDataList = new NetworkList<PlayerData>();

    

    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        network_PlayerDataList.Add(new PlayerData
        {
            clientId = clientId,
            playerColour = new(165, 165, 165),
        });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.PlayerCustomisation.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayerCount)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
        NetworkManager.Singleton.StartClient();
    }
    private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public int GetMaxPlayerCount()
    {
        return maxPlayerCount;
    }
    public void SetMaxPlayerCount(int inMaxPlayerCount)
    {
        maxPlayerCount = inMaxPlayerCount;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }
    
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < network_PlayerDataList.Count; i++)
        {
            if (network_PlayerDataList[i].clientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerData in network_PlayerDataList)
        {
            if(playerData.clientId == clientId)
            {
                return playerData;
            }
        }

        return default;
    }

    public PlayerData GetPlayerDataFromIndex(int playerIndex)
    {
        return network_PlayerDataList[playerIndex];
    }

    public Color GetPlayerColour(int colourId)
    {
        return playerColourList[colourId];
    }

    public void AddPlayerColourToList(Color inColour)
    {
        playerColourList.Add(inColour);
    }

    public void ChangePlayerColour(Color inColour)
    {
        ChangePlayerColour_ServerRpc(inColour);

    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColour_ServerRpc(Color inColour, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerColour = inColour;

        network_PlayerDataList[playerDataIndex] = playerData;
    }

    public void ChangePlayerName(FixedString32Bytes inName)
    {

    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerName_ServerRpc(FixedString32Bytes inName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerName = inName;

        network_PlayerDataList[playerDataIndex] = playerData;
    }
}
