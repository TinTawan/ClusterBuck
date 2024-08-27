using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton, createLobbyButton, quickJoinButton, refreshButton, joinPrivateButton;
    [SerializeField] private CreateLobbyUI createLobbyUI;
    [SerializeField] private TMP_InputField lobbyCodeInput;
     
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GetBuckedLobby.Instance.QuickJoin();
        });

        joinPrivateButton.onClick.AddListener(() =>
        {
            GetBuckedLobby.Instance.JoinByCode(lobbyCodeInput.text);
        });
    }
}
