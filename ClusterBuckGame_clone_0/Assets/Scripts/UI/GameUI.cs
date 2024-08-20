using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chargeValText;

    private PlayerScript playerScript;

    private void Awake()
    {
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (playerScript != null)
        {
            chargeValText.text = $"Charge Level: {playerScript.GetChargeLevel()}";
        }
        
    }

    private void OnEnable()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            playerScript = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<PlayerScript>();
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            playerScript = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerScript>();
        }
        chargeValText.text = "Charge Level: 0";
    }


}
