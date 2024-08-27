using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class GetBuckedLobby : MonoBehaviour
{

    public static GetBuckedLobby Instance { get; private set; }

    private Lobby joinedLobby;

    private float heartbeatTimer;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitialiseUnityAth();
    }

    private void Update()
    {
        HandleHeartBeat();
    }

    private void HandleHeartBeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0)
            {
                float maxHeartbeatTimer = 20f;
                heartbeatTimer = maxHeartbeatTimer;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }

    }
    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitialiseUnityAth()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();


        }

        
    }

    public async void CreateLobby(string lobbyName, bool isPrivate, int maxPlayerCount)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayerCount,
            new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            GetBuckedMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.PlayerCustomisation);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
        


    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            GetBuckedMultiplayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void JoinByCode(string code)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            GetBuckedMultiplayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }

    }
    
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
