using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.IO.Pipes;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] CoinWallet wallet;
    [SerializeField] Transform ProjectileSpawnPoint;
    [SerializeField] GameObject ServerProjectilePrefab;
    [SerializeField] GameObject ClientProjectilePrefab;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider playerCollider;
    

    [Header("Settings")]
    [SerializeField] float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] float muzzleFlashDuration;
    [SerializeField] private int costToFire; // if we have enough coins fire. 
    [SerializeField] AudioSource projectileAudioSource;
    [SerializeField] AudioClip projectileAudioClip;
    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;
    private bool useThirdPersonAiming;
    public override void OnNetworkSpawn()
    {
    
        // only subscribe to your own input
        if (!IsOwner) {  return; }
        inputReader.PrimaryFireEvent += InputReader_PrimaryFireEvent;
        PlayerMovement.OnTopDownEnter += UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter += UseThirdPersonAiming;

    }



    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= InputReader_PrimaryFireEvent;
        PlayerMovement.OnTopDownEnter += UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter += UseThirdPersonAiming;
    }
    // Update is called once per frame
    void Update()
    {
        // all users wshould be able to see it
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer < 0f) {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) { return; }
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        if (!shouldFire) {return; }

        // figure out when allowed to fire
        if(timer > 0) { return; }

        // enough to fire
        if(wallet.TotalCoins.Value < costToFire) { return; }

        Vector3 spawnPosition = ProjectileSpawnPoint.position;
        Vector3 fireDirection;
        if (useThirdPersonAiming == true) 
        {
             fireDirection = ProjectileSpawnPoint.forward.normalized;
        }
        else
        {
            fireDirection = ProjectileSpawnPoint.forward;
            fireDirection.y = 0f;
            fireDirection.Normalize();
        }
        
        // make sure server rpc getting same data so everyone can see projectile
        PrimaryFireServerRpc(spawnPosition, fireDirection);
        SpawnDummyProjectile(spawnPosition, fireDirection);

        timer = 1 / fireRate;
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
        if (wallet.TotalCoins.Value < costToFire) { return; }

        wallet.SpendCoins(costToFire);
        // trusting that client is firing
        GameObject projectileInstance = Instantiate(
            ServerProjectilePrefab,
            spawnPos,
            Quaternion.LookRotation(direction));

        

        //preventing projectile from colliding with s
        Physics.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider>());

        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            // whoever launcehd this projectile is the owner of it
            dealDamage.SetOwner(OwnerClientId);
        }

        if (projectileInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = direction.normalized * projectileSpeed;
        }
        SpawnAudioClip();
        SpawnDummyProjectileClientRpc(spawnPos, direction);
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
        ////preventing projectile from colliding with player for dummy as well
        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnAudioClip()
    {
        projectileAudioSource.PlayOneShot(projectileAudioClip);
    }
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // set muzzle flash since its part of visuals for projectile
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        // store object in variable
        GameObject projectileInstance = Instantiate(
            ClientProjectilePrefab,
            spawnPos,
            Quaternion.LookRotation(direction));

        

        // should also ingore collision for dummy projectile
        Physics.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider>());

                // in the event that the server and client projectile collides with itself it shouldn't destroy the players own projectile
            Physics.IgnoreCollision(ServerProjectilePrefab.GetComponent<Collider>(), ClientProjectilePrefab.GetComponent<Collider>());
        

        if(projectileInstance.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = direction.normalized * projectileSpeed;
        }
    }
    private void UseThirdPersonAiming()
    {
        useThirdPersonAiming = true;
    }

    private void UseTopDownAiming()
    {
        useThirdPersonAiming = false;
    }

}
