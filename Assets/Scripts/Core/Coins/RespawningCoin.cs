using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;

    private void Update()
    {
        
            if (transform.position != previousPosition)
            {
                Show(true);
            }
            previousPosition = transform.position;
        
    }
    public override int Collect()
    {
        if (IsServer)
        {
            if(alreadyCollected)
            {
                return 0;
            }
                alreadyCollected = true;

            OnCollected?.Invoke(this);

                return coinValue;
        }
        else
        {
            Show(false);
            return 0;
        }
    }

   

    public void Reset()
    {
       alreadyCollected = false; // will allow people to collect coins agian
    }
}
