using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [SerializeField] private BountyCoin coinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 10f;
    [SerializeField] private float bountyPercentage = 50; // bounty of coins dropped
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5; // coin count / min coin value, needs a minimum before spawning these coins
    [SerializeField] private LayerMask layerMask; // help to know where and where not bounty coins can spawn. 

    private float coinRadius;

    private Collider[] coinBuffer = new Collider[1];


    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CapsuleCollider>().radius;

        // listen for player death to spawn coin
        health.onDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer){ return; }

        health.onDie -= HandleDie;
    }

   

    public void SpendCoins(int coinsSpent)
    {
       TotalCoins.Value -= coinsSpent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Coin>(out Coin coin)) { return; }

        int storedCoinValue = coin.Collect();

        if (!IsServer) { return; }

        TotalCoins.Value += storedCoinValue;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));

        // how much should each coin be
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if(bountyCoinValue < minBountyCoinValue) { return; } // not enough to spawn

        for (int i = 0; i < bountyCoinCount; i++)
        {
           BountyCoin coinInstance =  Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private Vector3 GetSpawnPoint()
    {
        // if its safe to spawn spawn it if not then don't, could cause problems if there wasn't a spawn pos bit it should be fine
        // will probably need to modify these values better for 3D also y may need to be only 1 of 2 values due to how movement is locked.
        // 
        //TODO set one of 2 y positions for when the ships move later on
        float x = 0;
        float y = 0;
        float z = 0;
        while (true)
        {

            Vector3 spawnPoint = transform.position + UnityEngine.Random.insideUnitSphere * coinSpread; // keep it within offset of player
            spawnPoint.y = UnityEngine.Random.Range(-10, 0);// to prevent the coins spawning past the allowed y positon the ship can move in 
            // this math may be a bit off. also using var is a little slow 
            CapsuleCollider collider = coinPrefab.GetComponent<CapsuleCollider>();
            Vector3 direction = new Vector3 { [collider.direction] = 1 };
            var offset = collider.height / 2 - collider.radius;
            Vector3 localPoint0 = spawnPoint - direction * offset;
            Vector3 localPoint1 = spawnPoint + direction * offset;
            // using nonalloc is more performative as it wont allocate memory at runtime it will just reuse
            int numColliders = Physics.OverlapCapsuleNonAlloc(localPoint0, localPoint1, coinRadius, coinBuffer, layerMask);

            // keep looping and picking coordinates if there is anything in that space if nothing spawn a coin
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
