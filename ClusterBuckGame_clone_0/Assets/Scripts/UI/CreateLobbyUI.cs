using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private Button backButton, createLobbyButton;
    [SerializeField] private Toggle togglePrivate;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;
    [SerializeField] private TMP_InputField lobbyNameInput;


    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            GetBuckedLobby.Instance.CreateLobby(GetLobbyName(), togglePrivate.isOn, GetMaxPlayersFromDropdown());
        });
        backButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private int GetMaxPlayersFromDropdown()
    {
        return int.Parse(maxPlayersDropdown.captionText.text);
    }

    private string GetLobbyName()
    {
        if(lobbyNameInput.text != "")
        {
            return lobbyNameInput.text;
        }
        else
        {
            return $"Lobby {Random.Range(0, 100)}";
        }
    }
}
