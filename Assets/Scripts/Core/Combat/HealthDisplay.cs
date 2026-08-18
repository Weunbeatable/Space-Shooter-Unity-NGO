using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay: NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private Health health;

    [SerializeField] private Image healthBarImage;


    public override void OnNetworkSpawn()
    {
       if (!IsClient) { return; }
        health.currenthealth.OnValueChanged += HandleHealthChanged;
        // in case health subscribes after damage has been set, only once initially subscribed if it needs to be handled manually
        // will be set immediately. (common based on changed to due to an immediate subscription of the health value changing over the network)
        HandleHealthChanged(0, health.currenthealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }
        health.currenthealth.OnValueChanged -= HandleHealthChanged;
    }
    // network variables network syncs across old and new value so on callback its important to sync 
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        // health.currenthealth is the network value and onvaluechanged is an event fired across the network when the health 
        // value changes. its only important to subscribe for clients as its a UI change (not that important when theres a dedicated
        // server)
        healthBarImage.fillAmount = (float)newHealth/health.maxHealth;
    }
}
