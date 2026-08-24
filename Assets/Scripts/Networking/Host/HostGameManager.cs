using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : MonoBehaviour
{
    private Allocation allocation;
    private string joinCode;
    private const string GameSceneName = "Game";
    private const int MaxConnections = 20;
    //method to start as host which can be called through UI
    public async Task StartHostAsync()
    {
        try
        {

           allocation =  await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }
        // get people to join through join code
        try
        {

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
        // setup transport 
        // switch to relay mode and setup everything needed
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        Debug.Log($"NetworkManager: {NetworkManager.Singleton}");
        Debug.Log($"Transport: {transport}");
                // setup relay server data
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");  // dtls more secure than udp
        // but some players may not be able to use dtls 
        // UDP is unitys unity data gram protocol for server data
        // and handling connections. Can read more about udp in unity docs. 
        transport.SetRelayServerData(relayServerData);

        // tell it to start hosting
        NetworkManager.Singleton.StartHost();
        // change scene to go to gamepay scene.  call it based on scene name
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }
}
