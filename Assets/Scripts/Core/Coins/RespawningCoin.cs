using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public override int Collect()
    {
        if (IsServer)
        {
            if(alreadyCollected)
            {
                return 0;
            }
                alreadyCollected = true;
                return coinValue;
        }
        else
        {
            Show(false);
            return 0;
        }
    }

 
}
