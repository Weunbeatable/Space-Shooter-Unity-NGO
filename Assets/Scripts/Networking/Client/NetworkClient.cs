using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;
    private const string MenuSceneName = "Menu";
    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager; // caching network manager when it comes in 


        // using connectino approval
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }


    private void OnClientDisconnect(ulong clientId)
    {
     // will only be called when client disconnected from server. Called when any client disconnects
     // But need a gaurd as a host, if the host disconnects its because they left 
     if(clientId != 0 && clientId != networkManager.LocalClientId) { return; }
     // if disconnected if game shuts down, automatically disconnected. Need to then be routed to the right scene (main menu)

     if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }
        // if connected as a client then shutdown
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        // unsub from client disconnect
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= OnClientDisconnect;
        }
    }
}
