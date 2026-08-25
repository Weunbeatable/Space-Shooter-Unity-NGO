using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : MonoBehaviour
{
    private Allocation allocation;
    private string joinCode;
    private const string GameSceneName = "Game";
    private const int MaxConnections = 20;
    private string lobbyId;
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

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false; // if private wont show up in public queries but can still connect directly through code. 
            // will make toggleable later
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value:joinCode
                   )
                }
            };
            // need to send a ping to the lobby every few seconds to indicate that the lobby still exists and is active (heartbeat)
            // create the lobby
           Lobby lobby =  await Lobbies.Instance.CreateLobbyAsync(
               "My Lobby", MaxConnections, lobbyOptions);
            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        // tell it to start hosting
        NetworkManager.Singleton.StartHost();
        // change scene to go to gamepay scene.  call it based on scene name
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    // coroutine to heartbeat lobby
    // one issue is coroutines run on monobehaviors which exist in the scene but singletons dont so calling host singleton is a work around
    private IEnumerator HeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)    
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay; // wait 15 seconds and make ping 
        }
    }
}
/*lobby logic create allocation and create join code for it
 * set data on transport so ready for server
 * just before create lobby and join code to lobby
 * set 15 second interval when joining to server
 * the yield will allow for 15 second interval before pinging server. will stop as soon as game shuts down
 * will need to handle better later
 */
