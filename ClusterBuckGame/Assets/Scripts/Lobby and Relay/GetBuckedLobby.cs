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
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitialiseUnityAth();
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

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GetBuckedMultiplayer.Instance.GetMaxPlayerCount(),
            new CreateLobbyOptions
            {
                IsPrivate = isPrivate
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
    
}
