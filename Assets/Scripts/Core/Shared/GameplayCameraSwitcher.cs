using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameplayCameraSwitcher : NetworkBehaviour
{
    /// <summary>
    /// Purpose is so that players entering the boundaries of the switcher will have their camera settings updated ( useful when entering areas like tunnls, teleporting areas or dog fight zones
    /// to alter the gameplay feel. When leaving camera settings will be reverted back to the default top down settings. 
    /// </summary>
    public static Action<GameObject> OnThirdPersonEnter; //  subscribed to by player movement, player aiming AND a player camera as those are separate concerns 
    public static Action<GameObject> OnTopDownEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            OnThirdPersonEnter?.Invoke(other.attachedRigidbody.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            OnTopDownEnter?.Invoke(other.attachedRigidbody.gameObject);
        }
    }
}
