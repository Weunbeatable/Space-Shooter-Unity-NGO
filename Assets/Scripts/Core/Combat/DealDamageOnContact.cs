using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    // need to know projectile owner to ignore it

    private ulong ownerClientID;
    public void SetOwner (ulong ownerClientID)
    {
        //ulong for storing cliend ID's as they are only positive and very large
        this.ownerClientID = ownerClientID;
    }
    private void OnTriggerEnter(Collider other)
    {

        if(other.attachedRigidbody == null) { return; }

            if(other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netobj))
            {
                // if client we set locally is equal to the owner // means hit your own player. 
                if(ownerClientID == netobj.OwnerClientId)
                {
                    return;
                }
            }

            if(other.TryGetComponent<Health>(out Health health));
            {
                health.TakeDamage(damage);
            }
        
    }
}
