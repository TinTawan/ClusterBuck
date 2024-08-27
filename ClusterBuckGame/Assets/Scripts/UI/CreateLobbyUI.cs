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
            GetBuckedLobby.Instance.CreateLobby(lobbyNameInput.text, togglePrivate.isOn, maxPlayersDropdown.value);
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

}
