using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby, joinedLobby;

    private float heartBeatTimer, lobbyUpdateTimer;

    private string playerName;


    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

         playerName = "Tin" + UnityEngine.Random.Range(10, 99);

        //StartCoroutine(Test());
    }

    private void Update()
    {
        HandleLobbyHearbeat();
        //HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHearbeat()
    {
        if(hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartBeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    //check for and do any updates that happen to the lobby (players joining the lobby, changing name, changing colour etc.)
    private async void HandleLobbyPollForUpdates()
    {
        if (hostLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                float lobbyUpdateTimerMax = 1.5f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "TestLobby";
            int maxPlayers = 8;
            //can be a private or public lobby and also hold all player names
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                /*Player = new Player
                {
                    //this sets up the PlayerName Data, lets all members (only players in the lobby) see it, and asigns a random name to PlayerName
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
                }*/
                Player = GetPlayer(),
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);


            Debug.Log("Created lobby: Name: " + lobby.Name + " | Max Players: " + lobby.MaxPlayers + " | Lobby ID: " + lobby.Id + " | Lobby Code: " + lobby.LobbyCode);
            
            hostLobby = lobby;
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }

        
    }


    private async void ListLobbies()
    {
        
        try
        {
            //testing for now, use UI to allow player to change the options to what they need
            //most likely I will just have a lobby name filter
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                //filter amount of items to show after filtering
                Count = 5,
                //replace "0" with "x" where x is equal (EQ) to the name of the lobby
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.Name, "0", QueryFilter.OpOptions.EQ)
                },
                //true = ascending order, false = descending order, then showing them in order of when they were created (should only be 1 show up if using lobby name)
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };

            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log("Joined lobby with code: " + lobbyCode);

            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
            }
        };
            
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }

    }
    
    //remove this player from the current joined lobby
    //there is already auto host migration built in which sets the 2nd joined player as the host
    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    //the host can remove a certain player (using their id currently, but should change to kick using name or just have a button that allows them to kick them)
    private async void KickPlayer(string playerIdToKick)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerIdToKick);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    //delete lobby using a lobby ID
    private async void DeleteLobby(string lobbyIdToDelete)
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyIdToDelete);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e.Message);
        }
    }

    IEnumerator Test()
    {
        //ListLobbies();
        yield return new WaitForSeconds(2);
        CreateLobby();
        yield return new WaitForSeconds(1);
        ListLobbies();
        yield return new WaitForSeconds(1);
        //JoinLobbyByCode(lobbyCodeTest);

        /*yield return new WaitForSeconds(31);
        ListLobbies();*/

    }

}
