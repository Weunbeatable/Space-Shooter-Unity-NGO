using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;

public class ShipPlayer : NetworkBehaviour
{
    // should have references to both cameras or just one camera with its settings. 
    // can use scriptable objects to hold settings that should be used. 
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    // network variable that will sync over the network
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        // get user data 
        if (IsServer)
        {
           UserData userData = 
                HostSingleton.Instance.gameManager.networkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.userName; // creates a network sync so all other players can see this.
            // with a dediccated host this code will neeed to move
        }
        if (IsOwner)
        {
            // everyone has the same priority till spawned on network, but your own player will get higher priority 15
            // when spawned on network 
            cinemachineVirtualCamera.Priority = ownerPriority; 
        }
    }
}
