using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private ShipPlayer playerShip; 
    [SerializeField] private TMP_Text playerNameText; // reference to UI text that will be updated
    private void Start()
    {
        // unlikley case where subscription happens after a change has occurred 
        // force read of variable
        HandlePlayerNameChanged(string.Empty, playerShip.PlayerName.Value);
        playerShip.PlayerName.OnValueChanged += HandlePlayerNameChanged;
        
    }

    

    private void OnDestroy()
    {
        playerShip.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }
}
