using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    public Action<string> OnClientLeft;

    private Dictionary<ulong, string> clientIDToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

   

   public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager; // caching network manager when it comes in 

        
        // using connectino approval
        networkManager.ConnectionApprovalCallback += ApprovalCheck; // triggered when anyone connects to server and give us info about that ocnnection
        // handle disconnection to remove user data when they leave list
        networkManager.OnServerStarted += OnNetworkReady;
    }



    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        //paramaters are requests which is data coming in and response which is response to connection 
        // data sent over network is sent as a byte array, need to serialize, make a json object
        // then convert it to a byte array to send then convert it back to a json object deserialization

        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); // store string payload, contains object with name and other info gets bye array converted to json string
        UserData userData = JsonUtility.FromJson<UserData>(payload); // convert with from Json string to usable data

        // this payload is temporary, needs to be stored somewhere
        // when authenticated on client side need to know who they are. 
        // to ways to identify player, info user data passed in and client network id
        clientIDToAuth[request.ClientNetworkId] = userData.userAuthId; // store id along with user data
        authIdToUserData[userData.userAuthId] = userData;

        response.Approved = true; // lets users finish connection to server. 
        response.Position = SpawnPoints.GetRandomSpawnPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        // called once approval done, when server is ready to go
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIDToAuth.TryGetValue(clientId, out string authId))
        {
            clientIDToAuth.Remove(clientId); // data removed from dictionary

            authIdToUserData.Remove(authId); // removal from second dictionary

            //strategy when a client leaves a game to make room for new players to join lobby and
            // update lobby state
            OnClientLeft?.Invoke(authId); // go to lobby service and please remove user from lobby based on id
        }
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;

            if (networkManager.IsListening)
            {
                // if for somereason its still listening on disconnection shut it down
                networkManager.Shutdown();
            }
        }
    }

    public UserData GetUserDataByClientId(ulong clidentId)
    {
        if(clientIDToAuth.TryGetValue(clidentId, out string authId))
        {
            if(authIdToUserData.TryGetValue(authId, out  UserData data))
            {
                return data;
            }
            return null;
        }
        return null;
    }
}
