using System;
using TMPro;
using Unity.Collections;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainSection, colourSection, settingsSection;
    [SerializeField] private TextMeshProUGUI playerNameText, rVal, gVal, bVal, errorText;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Slider rSlider, gSlider, bSlider;
    [SerializeField] private SkinnedMeshRenderer playerMesh;

    private Color playerColour;
    private FixedString32Bytes playerName;

    [SerializeField] Button playButton, settingsButton, quitButton, colourButton, confirmButton;

    private void Awake()
    {
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
            ConfirmColour();
        });

        playerNameInputField.onValueChanged.AddListener(SetName);
    }

    private void Update()
    {
        if (colourSection.activeInHierarchy)
        {
            SetColour();
        }
    }

    private void ConfirmColour()
    {
        colourSection.SetActive(false);
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
        if (playerNameText.text.Length <= 0)
        {
            playerName = playerNameText.text;
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, 100);
            playerName = $"Deer {rand}";
        }
        //send playerName to network
        //send playerColour to network

        //move to lobby scene

    }

    public void BackToMenu(GameObject currentSection)
    {
        currentSection.SetActive(false);
        colourSection.SetActive(true);
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
        if (inName.Length <= 32)
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



    }


}
