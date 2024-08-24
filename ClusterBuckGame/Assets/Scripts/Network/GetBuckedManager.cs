using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GetBuckedManager : NetworkBehaviour
{
    public static GetBuckedManager Instance { get; private set; }

    private int maxPlayerCount = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

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

    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        /*if(SceneManager.GetActiveScene().name != Loader.Scene.MainMenu.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already started";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayerCount)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game full";
            return;
        }*/

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

}
