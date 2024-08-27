using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCustomisationUI : MonoBehaviour
{
    [Header("Player Customisation")]
    [SerializeField] private Button readyButton;
    [SerializeField] private Button menuButton, confirmButton, toLobbyButton, backToColourButton;
    [SerializeField] private Slider rSlider, gSlider, bSlider;
    [SerializeField] private TextMeshProUGUI rVal, gVal, bVal, playerNameText, errorText;
    [SerializeField] private TMP_InputField playerNameInputField;


    [SerializeField] private SkinnedMeshRenderer playerMesh;
    //[SerializeField] private Color bgColour, textColour;
    [SerializeField] GameObject lobbyPanel;

    private Material playerMaterial;
    private Color playerColour;
    private FixedString32Bytes playerName;


    [Header("Lobby Customisation")]
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyName2;
    [SerializeField] private TextMeshProUGUI lobbyCode;
    [SerializeField] private TextMeshProUGUI lobbyCode2;


    private void Awake()
    {
        menuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });
        readyButton.onClick.AddListener(() =>
        {
            PlayerCustomisationReady.Instance.SetPlayerReady();

        });
        confirmButton.onClick.AddListener(() =>
        {
            ConfirmPlayerData();
            ToLobby();

        });
        toLobbyButton.onClick.AddListener(() =>
        {
            ToLobby();

        });
        backToColourButton.onClick.AddListener(() =>
        {
            BackToColourSection();

        });

        playerMaterial = new Material(playerMesh.material);
        playerMesh.material = playerMaterial;

        playerNameInputField.onValueChanged.AddListener(SetName);

        rSlider.value = 165;
        gSlider.value = 165;
        bSlider.value = 165;
        errorText.enabled = false;

        readyButton.interactable = false;
        confirmButton.interactable = false;
        lobbyPanel.SetActive(false);

    }

    private void Start()
    {
        Lobby lobby = GetBuckedLobby.Instance.GetLobby();

        lobbyName.text = lobby.Name;
        lobbyCode.text = lobby.LobbyCode;

        lobbyName2.text = lobby.Name;
        lobbyCode2.text = lobby.LobbyCode;
    }


    private void BackToColourSection()
    {
        lobbyPanel.SetActive(!lobbyPanel.activeInHierarchy);
    }

    private void ToLobby()
    {
        lobbyPanel.SetActive(!lobbyPanel.activeInHierarchy);
    }

    private void Update()
    {
        SetColour();
        ConfirmButton();



    }
    private void ConfirmPlayerData()
    {
        playerName = playerNameText.text;
        //GetBuckedMultiplayer.Instance.AddPlayerColourToList(playerColour);
        readyButton.interactable = true;

        SetPlayerData();

        

    }

    private void ConfirmButton()
    {
        TextMeshProUGUI text = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmButton.IsInteractable())
        {
            text.color = Color.black;
        }
        else
        {
            text.color = Color.clear;
        }

        
    }

    private void SetPlayerData()
    {
       /* PlayerData thisPlayer = GetBuckedMultiplayer.Instance.GetPlayerData();
        thisPlayer.playerColour = playerColour;
        thisPlayer.playerName = playerName;*/

        GetBuckedMultiplayer.Instance.ChangePlayerName(playerName);
        GetBuckedMultiplayer.Instance.ChangePlayerColour(playerColour);

    }


    private void SetColour()
    {
        Color newColour = new Color32();
        newColour.r = rSlider.value / 255;
        newColour.g = gSlider.value / 255;
        newColour.b = bSlider.value / 255;

        rVal.text = rSlider.value.ToString();
        gVal.text = gSlider.value.ToString();
        bVal.text = bSlider.value.ToString();

        playerColour = newColour;

        playerMaterial.color = playerColour;
    }



    private void SetName(string inName)
    {
        if (inName.Length <= 16)
        {
            playerNameText.text = inName;
            errorText.enabled = false;

            playerNameText.color = Color.white;
            playerNameInputField.placeholder.color = Color.black;
            playerNameInputField.textComponent.color = Color.black;

            confirmButton.interactable = true;

        }
        else
        {
            //tell player name must be less than 32 characters
            playerNameText.text = "";
            playerNameInputField.placeholder.color = Color.red;
            playerNameInputField.textComponent.color = Color.red;
            errorText.enabled = true;
        }

        if (inName.Length == 0)
        {
            //assign random name to players who leave the input field blank
            int rand = UnityEngine.Random.Range(0, 100);
            playerNameText.text = $"Deer{rand}";
            errorText.enabled = false;

            playerNameText.color = Color.white;
            playerNameInputField.placeholder.color = Color.black;
            playerNameInputField.textComponent.color = Color.black;
        }

    }
}
