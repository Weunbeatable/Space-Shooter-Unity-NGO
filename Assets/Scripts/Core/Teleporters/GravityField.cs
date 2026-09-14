using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GravityField : NetworkBehaviour
{
    const float G = 667.4f;

    [SerializeField]public Rigidbody portalBody;
    private List<Rigidbody> playersInZone = new List<Rigidbody>();

    private void FixedUpdate()
    {
        if (!IsServer) { return; }
        if(playersInZone.Count == 0 ) { return; }
        foreach (Rigidbody body in playersInZone)
        {
            if(body != portalBody)
            Attract(body);
        }
    }
    public override void OnNetworkSpawn()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody == null) { return; }
        if (other.attachedRigidbody.gameObject.layer == 6) { return; } // if they are projectiles don't add them to the list. 
        // going forward there should be a list of layers that are excluded because as abilties and other objects are created they will need to be added to the list of exceptions 
        if (!other.TryGetComponent<Rigidbody>(out Rigidbody player)) { return; }
        {
            playersInZone.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody == null) { return; }
        if (other.attachedRigidbody.gameObject.layer == 6) { return; } // if they are projectiles don't add them to the list. 
        if (!other.TryGetComponent<Rigidbody>(out Rigidbody player)) { return; }
        {
            playersInZone.Remove(player);
        }
    }

    void Attract(Rigidbody shipBodies) // this may need to change if other objects with rigid bodies are added in
    {
        if (!IsServer) { return; }
        Rigidbody shipToPull = shipBodies.GetComponent<Rigidbody>();

        Vector3 direction = portalBody.position - shipToPull.position; // figure out direction the ship is in 
        float distance = direction.magnitude;

        float pullForceMagnitude =  G *(portalBody.mass * shipToPull.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * pullForceMagnitude;

        shipToPull.AddForce(force);
    }

}
