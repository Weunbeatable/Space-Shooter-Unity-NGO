using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using System;


public class PlayerCameraSettings : NetworkBehaviour
{

    [Header("Camera references")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera ThirdPersonCamera;
    [SerializeField] private CinemachineVirtualCamera BoostCamera; 
    [SerializeField] GameObject playerReference;
    public override void OnNetworkSpawn()
    {
        PlayerMovement.OnThirdPersonEnter += SetupThirdPersonCamera;
        PlayerMovement.OnTopDownEnter += SetupTopDownCamera;
    }

    

    public override void OnNetworkDespawn()
    {
        PlayerMovement.OnThirdPersonEnter -= SetupThirdPersonCamera;
        PlayerMovement.OnTopDownEnter -= SetupTopDownCamera;
    }

    private void SetupThirdPersonCamera()
    {
        if (!IsOwner) { return; }
        playerCamera.Priority = 14;
        ThirdPersonCamera.Priority = 15;
    }
    private void SetupTopDownCamera()
    {
        if (!IsOwner) { return; }
        ThirdPersonCamera.Priority = 14;
        playerCamera.Priority = 15;
        
    }
}
