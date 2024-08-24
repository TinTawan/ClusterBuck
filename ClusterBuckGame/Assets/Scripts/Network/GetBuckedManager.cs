using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetBuckedManager : NetworkBehaviour
{
    public static GetBuckedManager Instance {  get; private set; }


    [SerializeField] private Transform playerPrefab;

    private enum State
    {
        GamePlaying,
        GameOver,
    }


    private void Awake()
    {
        Instance = this;


    }


    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            //NetworkManager.Singleton.OnClientConnectedCallback += NetworkManger_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
        }
    }

    /*private void NetworkManger_OnClientDisconnectCallback(ulong clientId)
    {
        
    }*/
}
