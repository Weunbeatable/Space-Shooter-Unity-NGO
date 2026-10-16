using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilitySpawner : NetworkBehaviour
{
// when item spawns in, users cameras should up be updated
// item spawns in.
// once spawning complete, ping it on map,
// give up camera control. 
    private event Action InitiateAbilitySpawn;
    [SerializeField] private List<GameObject> abilityPrefab;

    [SerializeField] private int maxAbilitiesToSpawn = 20;

    [SerializeField] private int coinValue = 10;

    [SerializeField] private Vector3 xSpawnRange;

    [SerializeField] private Vector3 ySpawnRange;

    [SerializeField] private Vector3 zSpawnRange;

    [SerializeField] private LayerMask layerMask; // ensure coins aren't being spawned int he wrong places like walls or terrain. 

    private float abilityPrefabRadius;

    private float abilitySpawnTimer = 30f;

    private Collider[] coinBuffer = new Collider[1];
    // need to know how big coin is when spawning
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        // pick an ability from the list first
        int randomAbility =UnityEngine.Random.Range(0, abilityPrefab.Count - 1);

        // provide an array buffer to provide results, also just need to know if it collided with something not exactly what it was
        abilityPrefabRadius = abilityPrefab[randomAbility].GetComponent<CapsuleCollider>().radius;

        // spawn coins a bunch 
        for (int i = 0; i < maxAbilitiesToSpawn; i++)
        {
            SpawnAbility();
        }
    }
    private void SpawnAbility()
    {
        int randomAbility = UnityEngine.Random.Range(0, abilityPrefab.Count - 1);
        GameObject coinInstance = Instantiate(
            abilityPrefab[randomAbility],
            GetSpawnPoint(),
            Quaternion.identity);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        //listen in 
       // coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
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
            x = UnityEngine.Random.Range(xSpawnRange.x, xSpawnRange.z);
            y = UnityEngine.Random.Range(ySpawnRange.x, ySpawnRange.z);
            z = UnityEngine.Random.Range(zSpawnRange.x, zSpawnRange.z);
            Vector3 spawnPoint = new Vector3(x, y, z);

            // this math may be a bit off. also using var is a little slow 
            int randomAbility = UnityEngine.Random.Range(0, abilityPrefab.Count - 1);

            CapsuleCollider collider = abilityPrefab[randomAbility].GetComponent<CapsuleCollider>();
            Vector3 direction = new Vector3 { [collider.direction] = 1 };
            var offset = collider.height / 2 - collider.radius;
            Vector3 localPoint0 = spawnPoint - direction * offset;
            Vector3 localPoint1 = spawnPoint + direction * offset;
            // using nonalloc is more performative as it wont allocate memory at runtime it will just reuse
            int numColliders = Physics.OverlapCapsuleNonAlloc(localPoint0, localPoint1, abilityPrefabRadius, coinBuffer, layerMask);

            // keep looping and picking coordinates if there is anything in that space if nothing spawn a coin
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
