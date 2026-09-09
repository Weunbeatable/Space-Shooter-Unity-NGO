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
    [SerializeField] GameObject playerReference;
    public override void OnNetworkSpawn()
    {
        GameplayCameraSwitcher.OnThirdPersonEnter += SetupThirdPersonCamera;
        GameplayCameraSwitcher.OnTopDownEnter += SetupTopDownCamera;
    }

    

    public override void OnNetworkDespawn()
    {
        GameplayCameraSwitcher.OnThirdPersonEnter -= SetupThirdPersonCamera;
        GameplayCameraSwitcher.OnTopDownEnter -= SetupTopDownCamera;
    }

    private void SetupThirdPersonCamera(GameObject playerObject)
    {
        if (!IsOwner) { return; }
        playerCamera.Priority = 14;
        ThirdPersonCamera.Priority = 15;
    }
    private void SetupTopDownCamera(GameObject playerObject)
    {
        if (!IsOwner) { return; }
        ThirdPersonCamera.Priority = 14;
        playerCamera.Priority = 15;
        
    }
}
