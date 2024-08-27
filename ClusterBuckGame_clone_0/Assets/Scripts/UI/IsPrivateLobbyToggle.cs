using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

public class IsPrivateLobbyToggle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI publicText;

    private Toggle isPrivateToggle;

    private void Awake()
    {
        isPrivateToggle = GetComponent<Toggle>();

        isPrivateToggle.onValueChanged.AddListener(ToggleChanged);
    }

    private void ToggleChanged(bool toggle)
    {
        publicText.gameObject.SetActive(!toggle);
    }
}
