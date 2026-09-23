using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Reflector : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;
        if(other == this) return;
        if(other.TryGetComponent<Rigidbody>(out Rigidbody projectile))
        {
            if(projectile.gameObject.tag == "Projectile")
            {
                projectile.velocity = -projectile.velocity; // send it right back
            }
            else
            {
                projectile.velocity  = Vector3.Reflect(projectile.velocity, transform.forward); // richochet effect
            }
        }
    }


}
