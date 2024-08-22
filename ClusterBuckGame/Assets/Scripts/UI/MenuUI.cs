using System;
using TMPro;
using Unity.Collections;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MenuUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainSection;
    [SerializeField] private GameObject colourSection;
    [SerializeField] private GameObject settingsSection;
    [SerializeField] private TextMeshProUGUI playerNameText, rVal, gVal, bVal, errorText;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Slider rSlider, gSlider, bSlider;
    [SerializeField] private SkinnedMeshRenderer playerMesh;

    private Color playerColour;
    private FixedString32Bytes playerName;

    [SerializeField] private Button playButton, settingsButton, quitButton, colourButton, confirmButton;

    [Header("Lobby List")]
    [SerializeField] private Button backButton; 
    [SerializeField] private Button refreshButton, addLobbyButton, joinPrivateLobbyButton;
    [SerializeField] private InputField lobbyCodeInput;

    [Header("Joinable Lobby in List")]
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI currentPlayerCount, maxPlayerCount;

    [Header("Create Lobby")]
    [SerializeField] private Button returnToListButton;
    [SerializeField] private Button lobbyStartButton;
    [SerializeField] private InputField lobbyNameInputField;
    [SerializeField] private Toggle isPublicToggle;
    [SerializeField] private GameObject privateText;


    [Header("Player Card in Lobby")]
    [SerializeField] private TextMeshProUGUI playerLobbyName;
    [SerializeField] private Image playerLobbyColourImage;



    private void Awake()
    {
        //==== main menu ====
        mainSection.SetActive(true);
        colourSection.SetActive(false);
        settingsSection.SetActive(false);
        errorText.enabled = false;

        playButton.onClick.AddListener(() =>
        {
            PlayGame();
        });

        settingsButton.onClick.AddListener(() =>
        {
            OpenSettings();
        });

        quitButton.onClick.AddListener(() =>
        {
            QuitGame();
        });

        colourButton.onClick.AddListener(() =>
        {
            ColourSection();
        });

        confirmButton.onClick.AddListener(() =>
        {
            ConfirmButton();
        });

        playerNameInputField.onValueChanged.AddListener(SetName);

        rSlider.value = 165;
        gSlider.value = 165;
        bSlider.value = 165;

        //====


        //==== Create Lobby ====

        


        //====

    }

    private void Update()
    {
        if (colourSection.activeInHierarchy)
        {
            SetColour();
        }



        TogglePublicPrivate();
    }

    private void ConfirmButton()
    {
        colourSection.SetActive(false);
        settingsSection.SetActive(false);
        mainSection.SetActive(true);
    }

    private void ColourSection()
    {
        mainSection.SetActive(false);
        colourSection.SetActive(true);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OpenSettings()
    {
        mainSection.SetActive(false);
        settingsSection.SetActive(true);
    }

    private void PlayGame()
    {
        /*if (playerNameText.text.Length <= 0)
        {
            playerName = playerNameText.text;
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, 100);
            playerName = $"Deer {rand}";
        }*/

        //set the player name variable
        playerName = playerNameText.text;

        //send playerName to network
        //send playerColour to network

        //move to lobby scene

    }

    public void BackToMenu(GameObject currentSection)
    {
        currentSection.SetActive(false);
        mainSection.SetActive(true);
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

        playerMesh.material.color = playerColour;
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

        }
        else
        {
            //tell player name must be less than 32 characters
            playerNameText.text = "";
            playerNameInputField.placeholder.color = Color.red;
            playerNameInputField.textComponent.color = Color.red;
            errorText.enabled = true;
        }

        if(inName.Length == 0) 
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


    void TogglePublicPrivate()
    {
        if(isPublicToggle.isOn)
        {
            privateText.SetActive(false);
        }
        else
        {
            privateText.SetActive(true);
        }

    }

}
