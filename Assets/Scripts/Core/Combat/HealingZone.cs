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
    [SerializeField] private int MaxHealPower = 30; // how many times it can heal health
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healtTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10; //healing aint free dowg
    [SerializeField] private int healthPerTick = 10; // amount heal power will drop per heal tick
    [SerializeField] float _FillRateValue;
    [SerializeField] float progressBorder;
    [SerializeField] float stepSize;
    [SerializeField] private float progressSteps;

    private float remainingCooldown;
    private float tickTimer; // time to next tick of healing

    private List<ShipPlayer> playersInZone = new List<ShipPlayer>();  

    // server will be in charge of and dictate to clients
    private NetworkVariable<int> HealPower = new NetworkVariable<int>(); // clients need to subscribe to this

    public override void OnNetworkSpawn()
    {
        healMaterialFill = GetComponent<Renderer>().material;
        progressBorder = GetComponent<MeshFilter>().mesh.bounds.size.x / 2f; // set orientation accordingly
        healMaterialFill.SetFloat("_progressBorder", progressBorder);
        _FillRateValue = -progressBorder;
        // for progress bar
        healMaterialFill.SetFloat("_FillRtate", _FillRateValue); //initial value
        progressSteps = healthPerTick;
        stepSize = (2 * progressBorder) / progressSteps;

        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }

        if (IsServer)
        {
            // initalize heal power to mbe max value
            HealPower.Value = MaxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
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
                HealPower.Value = healthPerTick;
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

                HealPower.Value -= 1;

                if (HealPower.Value == 0)
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
        healMaterialFill.SetFloat("_progressBorder", (int)(newHealPower/MaxHealPower));

    }
}
