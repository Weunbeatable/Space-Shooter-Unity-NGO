using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnRipples : NetworkBehaviour
{
    public GameObject shieldRipples;

    private VisualEffect shieldRippilesVFX;

    private ulong ownerClientID;
    public void SetOwner(ulong ownerClientID)
    {
        //ulong for storing cliend ID's as they are only positive and very large
        this.ownerClientID = ownerClientID;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsOwner) { return; }

        if (other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject netobj))
        {
            // if client we set locally is equal to the owner // means hit your own player. 
            if (ownerClientID == netobj.OwnerClientId)
            {
                // if you hit yourself then don't do damage
                return;
            }
        }
        if(!other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rbobj)) { return; }
        {
           if( rbobj.tag == "Projectile");

            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRippilesVFX = ripples.GetComponent<VisualEffect>();
            shieldRippilesVFX.SetVector3("SphereCenter", other.contacts[0].point);

            Destroy(ripples, 2);
        }
    }

}
