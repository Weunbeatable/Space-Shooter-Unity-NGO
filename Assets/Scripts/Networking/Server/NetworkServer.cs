using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer 
{
    private NetworkManager networkManager;
   public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager; // caching network manager when it comes in 

        // using connectino approval
        networkManager.ConnectionApprovalCallback += ApprovalCheck; // triggered when anyone connects to server and give us info about that ocnnection
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

        Debug.Log(userData.userName);

        response.Approved = true; // lets users finish connection to server. 
        response.CreatePlayerObject = true;
    }
}
