using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton, joinGameButton;


    private void Awake()
    {
        //equivalent of the host pressing the Start button when inside a lobby
        createGameButton.onClick.AddListener(() =>
        {
            GetBuckedMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.PlayerCustomisation);
        });

        joinGameButton.onClick.AddListener(() =>
        {
            GetBuckedMultiplayer.Instance.StartClient();
        });
    }
}
