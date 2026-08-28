using Assets.Scripts.UI.Leaderboard;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColour;

    private FixedString32Bytes playerName;

    public ulong ClientId { get; private set; }
    
    public int Coins { get; private set; }


    public void Initalise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId;
        this.playerName = playerName;
        this.Coins = coins;

        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColour;
        }
        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }
    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {this.playerName} ({this.Coins})";
    }


}
