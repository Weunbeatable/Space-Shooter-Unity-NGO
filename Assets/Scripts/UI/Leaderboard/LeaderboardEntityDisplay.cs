using Assets.Scripts.UI.Leaderboard;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    private FixedString32Bytes playerName;

    public ulong ClientId { get; private set; }
    
    public int Coins { get; private set; }


    public void Initalise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        ClientId = clientId;
        this.playerName = playerName;
        this.Coins = coins;

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }
    private void UpdateText()
    {
        displayText.text = $"1. {this.playerName} ({this.Coins})";
    }


}
