using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerUI : NetworkBehaviour
{
    private Canvas canvas;
    private Camera cam;

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    private bool UiSet = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerName.Value = "Player " + OwnerClientId.ToString();

        }
        canvas = GetComponent<Canvas>();
        cam = FindObjectOfType<Camera>();

        canvas.worldCamera = cam;

    }

    private void Update()
    {
        if (!UiSet && !playerName.Value.IsEmpty)
        {
            SetOverlay();
        }

        transform.LookAt(canvas.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }

    public void SetOverlay()
    {
        TextMeshProUGUI localPlayerUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerUI.text = playerName.Value.ToString();

        UiSet = true;
    }


}
