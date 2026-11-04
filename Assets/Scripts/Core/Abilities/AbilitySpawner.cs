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
    public event Action<Transform, float> OnMaterializeAbility; // will pass a float for the time duration so other scripts can utilize it if necessary for how long to take over ship controls. 
    [SerializeField] private AudioSource AnnouncementSource;

    [SerializeField] private AudioClip supplyDropClip;

    [SerializeField] private AudioClip materializationClip;

    [SerializeField] private List<GameObject> abilityPrefab;

    [SerializeField] private int maxAbilitiesToSpawn = 20;

    [SerializeField] private int coinValue = 10;

    [SerializeField] private Vector3 xSpawnRange;

    [SerializeField] private Vector3 ySpawnRange;

    [SerializeField] private Vector3 zSpawnRange;

    [SerializeField] private LayerMask layerMask; // ensure coins aren't being spawned int he wrong places like walls or terrain. 
    [SerializeField] private float minTimerValue;

    [SerializeField] private float maxTimerValue;


    [SerializeField] private float abilitySpawnTimer;

    [SerializeField] public float materializationDelayTimer = 0f;

    private float announcementTimer;
    private float abilityPrefabRadius;

    private Collider[] coinBuffer = new Collider[1];

    public NetworkObject materializedAbilityNetworkObject;

    private Coroutine spawnSequence;

    public static AbilitySpawner Instance;
    // need to know how big coin is when spawning


    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("An instance already exists " + this.name);
            return;
        }
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        // add singleton 
        if(Instance != this)
        {
            Debug.Log("Invalid ability spawer instance created");
            return;
        }


        if (!IsServer) { return; }

        OnMaterializeAbility += InitiateMaterialization;

        spawnSequence = StartCoroutine(SpawnSequence());

        // pick an ability from the list first
        int randomAbility = UnityEngine.Random.Range(0, abilityPrefab.Count - 1);

        // provide an array buffer to provide results, also just need to know if it collided with something not exactly what it was
        //abilityPrefabRadius = abilityPrefab[randomAbility].GetComponent<CapsuleCollider>().radius;

        // spawn coins a bunch 
        //for (int i = 0; i < maxAbilitiesToSpawn; i++)
        //{
        //    SpawnAbility();
        //}
    }

    public override void OnNetworkDespawn()
    {
        if(Instance == this)
        {
            Instance = null;
        }
        if (spawnSequence != null)
        {
            StopCoroutine(spawnSequence);
            spawnSequence = null;
        }
        base.OnNetworkDespawn();
    }

    private void InitiateMaterialization(Transform abilityTransform, float delayTimer)
    {
        AnnouncementSource.PlayOneShot(materializationClip);
    }

    private IEnumerator SpawnSequence()
    {
        // initial timer wait
        announcementTimer = UnityEngine.Random.Range(minTimerValue, maxTimerValue);

        yield return new WaitForSeconds(announcementTimer);

        // play audio suply drop 
        PlaySupplyDropAnnounncementClientRpc();

        // wait before an actual spawn starts
        abilitySpawnTimer = UnityEngine.Random.Range(minTimerValue, maxTimerValue);

        yield return new WaitForSeconds(abilitySpawnTimer);

        // Spawn ability and notify listeners
        SpawnAbility();

        spawnSequence = null;
    }

    private void Update()
    {
       if(!IsServer) { return; }

    }
    private void SpawnAbility()
    {
        if (!IsServer) { return; }

        int randomAbility = UnityEngine.Random.Range(0, abilityPrefab.Count - 1);

        // need to collider for validatio to prevent stacking an ability in the same place.

        GameObject fab  = abilityPrefab[randomAbility];


        GameObject abilityInstance = Instantiate(
            abilityPrefab[randomAbility],
            GetSpawnPoint(),
            Quaternion.identity);

        NetworkObject abilityNetworkObject = abilityInstance.GetComponent<NetworkObject>();

        if (NetworkObject == null)
        {
            Destroy(abilityInstance); return;
        }

        //server nentwork spawn
        abilityNetworkObject.Spawn();

       
        // provide new transform data so i can pass the positional data to other scripts that need it including transform data. 
        NotifyMaterializationClientRpc(
            new NetworkObjectReference(abilityNetworkObject), 
            materializationDelayTimer);
        materializedAbilityNetworkObject = abilityNetworkObject;
        //listen in 
        if (IsServer)
        {
            ShipPlayer[] players = FindObjectsByType<ShipPlayer>(FindObjectsSortMode.None);
            foreach (ShipPlayer player in players)
            {
                player.abilityController.TriggerPresentation(materializedAbilityNetworkObject, materializationDelayTimer);
            }
        }
        // once an object is spawned then pass it into the event. 
        OnMaterializeAbility?.Invoke(materializedAbilityNetworkObject.transform, materializationDelayTimer);
    }


    [ClientRpc]
    private void PlaySupplyDropAnnounncementClientRpc()
    {
        if (AnnouncementSource != null && supplyDropClip != null)
        {
            AnnouncementSource.PlayOneShot(supplyDropClip);
        }
    }


    [ClientRpc]
    private void NotifyMaterializationClientRpc(NetworkObjectReference networkObjectReference, float materializationDelayTimer)
    {
        if(!networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.LogWarning("Couldn't respolve the spawned ability Network Object" + this);
            return;
        }

        Transform abilityTransform = networkObject.transform;

        if (AnnouncementSource != null && materializationClip != null)
        {
            AnnouncementSource.PlayOneShot(materializationClip);
        }
        // execute locally on each client
        OnMaterializeAbility?.Invoke(abilityTransform, materializationDelayTimer);
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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
