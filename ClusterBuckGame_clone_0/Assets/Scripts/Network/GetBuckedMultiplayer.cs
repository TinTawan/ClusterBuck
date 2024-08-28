using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using Unity.Collections;
using Unity.Services.Authentication;

public class GetBuckedMultiplayer : NetworkBehaviour
{
    public static GetBuckedMultiplayer Instance { get; private set; }

    private const string PlayerPrefs_PlayerName_Multiplayer = "PlayerNameMultiplayer";
    private const string PlayerPrefs_PlayerColour_Multiplayer = "PlayerColourMultiplayer";


    //private int maxPlayerCount = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataListChanged;

    private NetworkList<PlayerData> network_PlayerDataList;

    //private List<Color> playerColourList;

    private string playerName, playerColourHex;
    private Color playerColourOut;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        network_PlayerDataList = new NetworkList<PlayerData>();
        network_PlayerDataList.OnListChanged += PlayerDataList_OnListChanged;

        playerName = PlayerPrefs.GetString(PlayerPrefs_PlayerName_Multiplayer, "Deer " + UnityEngine.Random.Range(0, 1000));
        playerColourHex = PlayerPrefs.GetString(PlayerPrefs_PlayerColour_Multiplayer, "#A4A4A4");


    }

    private void PlayerDataList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataListChanged?.Invoke(this, EventArgs.Empty);   
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetPlayerName(string inName)
    {
        playerName = inName;

        PlayerPrefs.SetString(PlayerPrefs_PlayerName_Multiplayer, inName);
    }

    public Color GetPlayerColour()
    {
        if (ColorUtility.TryParseHtmlString(playerColourHex, out playerColourOut))
        {
            return playerColourOut;
        }

        return default;
    }
    public string GetPlayerColourByHex()
    {
        return playerColourHex;
    }
    public void SetPlayerColourHex(Color inColour)
    {
        SetPlayerColourHex(ColorUtility.ToHtmlStringRGB(inColour));
    }
    public void SetPlayerColourHex(string inHexCode)
    {
        playerColourHex = $"#{inHexCode}";

        PlayerPrefs.SetString(PlayerPrefs_PlayerColour_Multiplayer, inHexCode);
    }


    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();

        
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < network_PlayerDataList.Count; i++)
        {
            PlayerData playerData = network_PlayerDataList[i];
            if (playerData.clientId == clientId)
            {
                network_PlayerDataList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        network_PlayerDataList.Add(new PlayerData
        {
            clientId = clientId,
            //playerColour = new(165, 165, 165),
            playerHexColour = "#A4A4A4"
        });

        SetPlayerName_ServerRpc(GetPlayerName());
        SetPlayerId_ServerRpc(AuthenticationService.Instance.PlayerId);

        SetPlayerColourByHex(GetPlayerColourByHex());
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.PlayerCustomisation.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already started";
            return;
        }

        if (GetBuckedLobby.Instance.IsLobbyFull())
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

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectedCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerName_ServerRpc(GetPlayerName());
        SetPlayerId_ServerRpc(AuthenticationService.Instance.PlayerId);

        SetPlayerColourByHex(GetPlayerColourByHex());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerName_ServerRpc(string inName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerName = inName;

        network_PlayerDataList[playerDataIndex] = playerData;

    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerId_ServerRpc(string inId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerName = inId;

        network_PlayerDataList[playerDataIndex] = playerData;

    }


    private void NetworkManager_Client_OnClientDisconnectedCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    /*public int GetMaxPlayerCount()
    {
        return maxPlayerCount;
    }
    public void SetMaxPlayerCount(int inMaxPlayerCount)
    {
        maxPlayerCount = inMaxPlayerCount;
    }*/

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

    /*public Color GetPlayerColour(int colourId)
    {
        return playerColourList[colourId];
    }

    public void AddPlayerColourToList(Color inColour)
    {
        playerColourList.Add(inColour);
    }*/

    public void SetPlayerColourByHex(string hexCode)
    {
        SetPlayerColourByHex_ServerRpc(hexCode);

    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerColourByHex_ServerRpc(string hexCode, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerHexColour = $"#{hexCode}";

        network_PlayerDataList[playerDataIndex] = playerData;
    }


    /*public void SetPlayerColour(Color inColour)
    {
        SetPlayerColour_ServerRpc(inColour);

    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerColour_ServerRpc(Color inColour, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerColour = inColour;

        network_PlayerDataList[playerDataIndex] = playerData;

    }*/

    /*public void ChangePlayerName(FixedString32Bytes inName)
    {
        ChangePlayerName_ServerRpc(inName);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerName_ServerRpc(FixedString32Bytes inName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = network_PlayerDataList[playerDataIndex];
        playerData.playerName = inName;

        network_PlayerDataList[playerDataIndex] = playerData;
    }*/



}
