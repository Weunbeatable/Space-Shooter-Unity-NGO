using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShieldsDemo : NetworkBehaviour
{
    [SerializeField] Health shieldHealth;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        shieldHealth.onDie += DestroyShields;
    }

    

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        shieldHealth.onDie -= DestroyShields;
    }
    private void DestroyShields(Health health)
    {
        if (!IsOwner) { return; }
        Destroy(this.gameObject);
    }
}
