using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CleanUp : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (GetBuckedMultiplayer.Instance != null)
        {
            Destroy(GetBuckedMultiplayer.Instance.gameObject);
        }

        if (GetBuckedLobby.Instance != null)
        {
            Destroy(GetBuckedLobby.Instance.gameObject);
        }
    }
}
