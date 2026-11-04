using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System;

public class ShipPlayer : NetworkBehaviour
{
    // should have references to both cameras or just one camera with its settings. 
    // can use scriptable objects to hold settings that should be used. 
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] CinemachineVirtualCamera thirdPersonCam;
    [SerializeField] CinemachineVirtualCamera announcementCamera; 
    [SerializeField] private SpriteRenderer minimapIconRenderer;
    [SerializeField] AbilitySpawner spawnerInstance;

    [Header("Settings")]
    [SerializeField] public int ownerPriority { get; set; } = 15;
    [SerializeField] public int ownerSeconndaryPriority { get; set; } = 14;

    [SerializeField] public int announcementPriority { get; set; } = 11;

    [SerializeField] private Color ownerColor;
    [field: SerializeField] public Rigidbody playerBody { get; set; }
    [field: SerializeField] public Health Health { get; set; } // field will leave it exposed in editor without open to modification
    [field: SerializeField] public CoinWallet Wallet { get; set; } // field will leave it exposed in editor without open to modification

    [field: SerializeField] public AbilityPresentationController abilityController { get; set; } // field will leave it exposed in editor without open to modification
    // network variable that will sync over the network
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    //static event to broadcast when a player is spawned in 
    public static event Action<ShipPlayer> OnPlayerSpawned;

    public static event Action<ShipPlayer> OnPlayerDespawned;
    public override void OnNetworkSpawn()
    {
        // get user data 
        if (IsServer)
        {
           UserData userData = 
                HostSingleton.Instance.gameManager.networkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.userName; // creates a network sync so all other players can see this.
            // with a dediccated host this code will neeed to move

            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            // everyone has the same priority till spawned on network, but your own player will get higher priority 15
            // when spawned on network 
            spawnerInstance = AbilitySpawner.Instance;
            //spawnerInstance.onm
            cinemachineVirtualCamera.Priority = ownerPriority;
            thirdPersonCam.Priority = ownerSeconndaryPriority;
            announcementCamera.Priority = announcementPriority;

            minimapIconRenderer.color = ownerColor;
            abilityController.OnMaterializationPresentation += PresentationControlsSetup;
        }
    }

    private void PresentationControlsSetup(Transform transform, float delayTime)
    {
        StartCoroutine(AbilityPresenting(transform, delayTime));
    }

    public IEnumerator AbilityPresenting(Transform AbilityToFocusOn, float waitTime)
    {
        // small chance it spawns on someone and they immediately get the ability which essentially means there is
        // nothing to focus on
        //if(AbilityToFocusOn == null) { yield return new WaitForSeconds(0f); }
        if (AbilityToFocusOn != null)
        {
            announcementCamera.Follow = AbilityToFocusOn;
            announcementCamera.LookAt = AbilityToFocusOn;
            announcementPriority = 18;
            announcementCamera.Priority = announcementPriority;
            Health.SetInvulnerability(true);
        }
        yield return new WaitForSeconds(waitTime);
        announcementPriority = 11;
        announcementCamera.Priority = announcementPriority;
        Health.SetInvulnerability(false);
        Debug.Log($"Invulnerability state is set to {Health.IsInvulnerable()}");
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
        if (IsOwner)
        {
            abilityController.OnMaterializationPresentation -= PresentationControlsSetup;
        }
    }

    public void RequestToTeleportClientShip(Vector3 destinationToTeleportTo)
    {
        if(!IsServer) { return; }

        // create client rpc object params
        ClientRpcParams clientRpcShip = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]
                {
                    OwnerClientId,
                }
            }
        };
       TeleportPlayerShipClientRpc(destinationToTeleportTo, clientRpcShip);
    }

    [ClientRpc]
    private void TeleportPlayerShipClientRpc(Vector3 location, ClientRpcParams playerShip = default)
    {
        playerBody.position = location;
    }


}
