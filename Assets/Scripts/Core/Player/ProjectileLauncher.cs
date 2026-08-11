using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform ProjectileSpawnPoint;
    [SerializeField] GameObject ServerProjectilePrefab;
    [SerializeField] GameObject ClientProjectilePrefab;

    [Header("Settings")]
    [SerializeField] float projectileSpeed;
    private bool shouldFire;
    public override void OnNetworkSpawn()
    {
    
        // only subscribe to your own input
        if (!IsOwner) {  return; }
        inputReader.PrimaryFireEvent += InputReader_PrimaryFireEvent;

    }

    

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= InputReader_PrimaryFireEvent;
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        
        if (!shouldFire) {return; }

        // make sure server rpc getting same data so everyone can see projectile
        PrimaryFireServerRpc(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);
        SpawnDummyProjectile(ProjectileSpawnPoint.position, ProjectileSpawnPoint.up);
    }

  
    private void InputReader_PrimaryFireEvent(bool shouldFire)
    {
        // the bool will allow for hold to fire
        this.shouldFire = shouldFire;
    }
    // server rpc to know about firing
    //name of function must end with ServerRpc and have ServerRpc attriute on top
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // trusting that client is firing
        GameObject projectileInstance = Instantiate(
            ServerProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction;

        SpawnDummyProjectile(spawnPos, direction);
    }


    // client calls a server rpc and server should call clientRpc
    //spawn dummy projectile client rpc
    // apply attribute
    // call from server code
    // call spawn dummy projectile if not owner
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) { return; }
        SpawnDummyProjectile(spawnPos, direction);
    }
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // store object in variable
        GameObject projectileInstance = Instantiate(
            ClientProjectilePrefab,
            spawnPos,
            Quaternion.identity);

        projectileInstance.transform.up = direction;
    }

  
}
