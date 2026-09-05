using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour
{
   public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // this will handle shutting down the server as a host
            HostSingleton.Instance.gameManager.Shutdown();
             
            //but if you're a client you need a way to leave, also hosts are types of clients
            // so they need to be able to safely leave as well. 
        }

        // case for when player wants to leave and hits leave button 
        ClientSingleton.Instance.GameManager.Disconnect();
    }
}
