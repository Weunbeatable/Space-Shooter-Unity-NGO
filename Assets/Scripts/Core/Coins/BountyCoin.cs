using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;
    private Vector3 previousPosition;

 
    public override int Collect()
    {
        if (IsServer)
        {
            if (alreadyCollected)
            {
                return 0;
            }
            alreadyCollected = true;

            Destroy(gameObject);

            return coinValue;
        }
        else
        {
            Show(false);
            return 0;
        }
    }


}
