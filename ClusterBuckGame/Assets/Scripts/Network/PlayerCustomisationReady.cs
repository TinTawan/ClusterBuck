using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class PlayerCustomisationReady : NetworkBehaviour
{
    public static PlayerCustomisationReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();

    }

    public void SetPlayerReady()
    {
        SetPlayerReady_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReady_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //This player is not ready
                allClientsReady = false;
                break;
            }
        }

        if(allClientsReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
}
