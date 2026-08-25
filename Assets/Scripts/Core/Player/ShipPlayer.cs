using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class ShipPlayer : NetworkBehaviour
{
    // should have references to both cameras or just one camera with its settings. 
    // can use scriptable objects to hold settings that should be used. 
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // everyone has the same priority till spawned on network, but your own player will get higher priority 15
            // when spawned on network 
            cinemachineVirtualCamera.Priority = ownerPriority; 
        }
    }
}
