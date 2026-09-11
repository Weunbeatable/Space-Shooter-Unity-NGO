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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            player.TriggerThirdPersonMode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            player.TriggerTopDownMode();
        }
    }
}
