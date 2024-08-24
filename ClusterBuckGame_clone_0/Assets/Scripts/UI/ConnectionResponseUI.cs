using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMessage;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        GetBuckedMultiplayer.Instance.OnFailedToJoinGame += GetBuckedMultiplayer_OnFailedToJoin;

        Hide();
    }

    private void GetBuckedMultiplayer_OnFailedToJoin(object sender, EventArgs e)
    {
        Show();

        textMessage.text = NetworkManager.Singleton.DisconnectReason;

        if(textMessage.text == "")
        {
            textMessage.text = "Failed to connect";
        }
    }

    void Show()
    {
        gameObject.SetActive(true);

    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GetBuckedMultiplayer.Instance.OnFailedToJoinGame -= GetBuckedMultiplayer_OnFailedToJoin;
    }
}
