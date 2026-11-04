using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityPresentationController : NetworkBehaviour
{
    public event Action<Transform, float> OnMaterializationPresentation;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
    }
    internal void TriggerPresentation(NetworkObject materializedAbility, float materializationDelayTimer)
    {
        if (!IsServer) { return; }

        if(materializedAbility == null || !materializedAbility.IsSpawned)
        {
            Debug.LogWarning("Can't start presentation: ability NetworkObject is null or missing");
            return;
        }
        NetworkObjectReference abilityRefrence = new NetworkObjectReference(materializedAbility);

        // send message to clients that observe this controllers Network OBject; 
    

        TriggerPresentationControllerCallClientRpc( materializedAbility, materializationDelayTimer);
    }

    [ClientRpc]
    private void TriggerPresentationControllerCallClientRpc( NetworkObjectReference abilityRefrence, float DelayTimer,ClientRpcParams ship = default)
    {
        if(abilityRefrence.TryGet(out NetworkObject abilityNetworkObject))
        {
            // turn the reference into a local transform.
            Transform abilityTransform = abilityNetworkObject.transform;

            // notify local presentation related scripts
            OnMaterializationPresentation?.Invoke(abilityTransform, DelayTimer);
        }

        else
        {
            Debug.LogWarning("Ability reference couldn't be resolved on this client. " + "Ability may not have been locally spawned yet.");
        }
    }
}
