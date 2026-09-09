using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Material healMaterialFill;

    [Header("Settings")]
    [SerializeField] private int MaxHealPower = 90; // how many times it can heal health
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healtTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10; //healing aint free dowg
    [SerializeField] private int healthPerTick = 10; // amount heal power will drop per heal tick
    [SerializeField] float progressBorder;
    [SerializeField]private float progressBorderValue = 0.57f;

    private float remainingCooldown;
    private float tickTimer; // time to next tick of healing

    private List<ShipPlayer> playersInZone = new List<ShipPlayer>();  

    // server will be in charge of and dictate to clients
    private NetworkVariable<int> HealPower = new NetworkVariable<int>(); // clients need to subscribe to this

    public override void OnNetworkSpawn()
    {
        //healMaterialFill = GetComponent<Renderer>().material;
        progressBorder = progressBorderValue;
        healMaterialFill.SetFloat("_ProgressBorder", progressBorder);


        if (IsServer)
        {
            // initalize heal power to mbe max value
            HealPower.Value = MaxHealPower;
        }

        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(HealPower.Value, HealPower.Value);
        }

        
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
            if (!other.attachedRigidbody.TryGetComponent<ShipPlayer>(out ShipPlayer player)){ return; }
            {
                playersInZone.Add(player);
            }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
            if (!other.attachedRigidbody.TryGetComponent<ShipPlayer>(out ShipPlayer player)){ return; }
            {
                playersInZone.Remove(player);
            }
    }

    private void Update()
    {
        // check every frame whos in zone and heall power etc
        if(!IsServer) { return; }

        if(remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;

            if (remainingCooldown <= 0f)
            {
                HealPower.Value = MaxHealPower;
            }
            else { return; }
        }
        // use tick rate to know if you should heal or wait  
        tickTimer += Time.deltaTime;
        // chec if ready to do tick
        if (tickTimer >= 1 / healtTickRate)
        {
            foreach (ShipPlayer player in playersInZone)
            {
                if (HealPower.Value == 0) { break; }

                // check if pllayer is at max health
                if (player.Health.currenthealth.Value == player.Health.maxHealth) { continue; }

                // can't heal broke boys
                if (player.Wallet.TotalCoins.Value < coinsPerTick) { continue; }

                // meet criteria for healing
                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);

                HealPower.Value = Mathf.Max(0, HealPower.Value - healthPerTick);

                if (HealPower.Value <= 0)
                {
                    remainingCooldown = healCooldown;
                }
            }

            // a remainder for how much overshot by timer reaching 0 is hard to do with time.deltatime
            tickTimer = tickTimer % (1 / healtTickRate); 

        }
    }
    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        // to account for integer divison not working
        float normalizedHealPower = (float)newHealPower / MaxHealPower;

        // lerp will allow for a smooth transition in the material visual when updating the healing power
        // left 
        float fillRate = Mathf.Lerp(-progressBorder, progressBorder, normalizedHealPower);

        healMaterialFill.SetFloat("_FillRate", fillRate); // fill rate is what modifies depending on
        // the healing power left over. 
        // based off current settings I have heal power max = 3, tick -5 so
        // 25/30 = 0.83 so for fill rate it goes 0.57, ~0.38 etc 
        //NOTE if progress bar changes so do these values need to change. 

    }
}
