using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{


    private void Start()
    {
        GetBuckedMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoin;
        GetBuckedMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoin;

        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoin(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoin(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
       
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GetBuckedMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoin;
        GetBuckedMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoin;
    }
}
