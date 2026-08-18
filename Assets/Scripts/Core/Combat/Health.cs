using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // when health changes want all clients to know about it so network behaviour and network vars are used instead of just mono

    [field: SerializeField] public int maxHealth { get; private set; } = 100; // since this doesn't change it doesn't need to be a network variable
    public NetworkVariable<int> currenthealth = new NetworkVariable<int>(); // variable is only modifiable by server
    // once modiified other players will see the change as well

    // ensure death doesn't trigger twice in case something happens on the network
    private bool isDead;

    public Action<Health> onDie;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) {return;}

        currenthealth.Value = maxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    // function that will actually interact with health value
    private void ModifyHealth(int value)
    {
        if (isDead) return;

        int newHealth = currenthealth.Value + value;
        currenthealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        if (currenthealth.Value == 0)
        {
            onDie?.Invoke(this);
            isDead = true;
        }
    }
}
