using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private ShipPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage;

    public override void OnNetworkSpawn()
    {
        // as soon as respawn handler starts to exist (when arriving at gameplay scene) 
        if (!IsServer) { return; }
        // Situations where player goes before scene (like self hosting) 
        ShipPlayer[] players = FindObjectsByType<ShipPlayer>(FindObjectsSortMode.None);
        foreach (ShipPlayer player in players)
        {
           HandlePlayerSpawned(player);
        }

        // listen to health scripts on event calls

        ShipPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        ShipPlayer.OnPlayerSpawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        ShipPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        ShipPlayer.OnPlayerSpawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(ShipPlayer player)
    {
        player.Health.onDie += (health) => HandlePlayerDie(player); // lambda to pass in parameters and handle player normally
    }
    private void HandlePlayerDespawned(ShipPlayer player)
    {
        player.Health.onDie -= (health) => HandlePlayerDie(player); // lambda to pass in parameters , will stop listening here when player dies
    }

  
    private void HandlePlayerDie(ShipPlayer player)
    {
        int keptCoins = (int) (player.Wallet.TotalCoins.Value * (keptCoinPercentage / 100));
        Destroy(player.gameObject);

        // wait till next frame before spawning player again 
        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
    {

        yield return null;

      ShipPlayer playerInstance = 
            Instantiate(playerPrefab, SpawnPoints.GetRandomSpawnPos(), Quaternion.identity);

        // assign newly spawned object to the owner client id so the new ship belowngs to player 
        // order for coin update matters dont want to modify coins value before spawned in as network object
        playerInstance.Wallet.TotalCoins.Value += (int)keptCoins;
        playerInstance.NetworkObject. SpawnAsPlayerObject(ownerClientId);
    }

   
}
