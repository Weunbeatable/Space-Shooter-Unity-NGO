using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager 
{
    private const string MenuSceneName = "Menu";
    private JoinAllocation allocation;
    // lasts an unkown amount of time can be there till task is done
    public async Task<bool> InitiAsync() // to know if whoever initalized this function was successful turn it into a bool task
    {
        // authenticate player
        // init unity services
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        // let whoever called gotomenu find out the end result
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {

        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception relayException)
        {
            Debug.LogError(relayException);
            return; // if trying to join goes wrong we return otherwise continue by getting up transprt component and setting up allocation data
        }
        // get refrence to transport component in order to setup data for it before connecting
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();


        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");  // dtls more secure than udp
        transport.SetRelayServerData(relayServerData);

        //set data before starting the client
        UserData userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.playerNameKey, "Missing Name") // if somehow didn't get a name put missing name
        };
        // reverse json conversion method converting to Json not from
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes; // sent when connected to server 

        // tell it to start hosting
        NetworkManager.Singleton.StartClient();
    }
}
