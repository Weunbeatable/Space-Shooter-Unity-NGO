using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    // when a coin is collected, need to tell server, so when collected hide it client side and then when server confirms
    // can reenable

    [SerializeField] private MeshRenderer meshRenderer;

    protected int coinValue;
    protected bool alreadyCollected; // in case 2 players walk over the same coin at the same time

    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        meshRenderer.enabled = show;
    }

}
