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
        if(other.attachedRigidbody == null) { return; }
        if (other.attachedRigidbody.gameObject.layer == 6) { return; } // if they are projectiles don't add them to the list. 
        // going forward there should be a list of layers that are excluded because as abilties and other objects are created they will need to be added to the list of exceptions 
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            player.TriggerThirdPersonMode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody == null) { return; }
        if (other.attachedRigidbody.gameObject.layer == 6) { return; } // if they are projectiles don't add them to the list. 
        // going forward there should be a list of layers that are excluded because as abilties and other objects are created they will need to be added to the list of exceptions 
        if (!other.attachedRigidbody.TryGetComponent<PlayerMovement>(out PlayerMovement player)) { return; }
        {
            player.TriggerTopDownMode();
        }
    }
}
