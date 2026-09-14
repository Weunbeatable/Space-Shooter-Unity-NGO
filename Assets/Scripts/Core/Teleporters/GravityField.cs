using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GravityField : NetworkBehaviour
{
    const float G = 667.4f;

    public Rigidbody portalBody;
    private List<Rigidbody> playersInZone = new List<Rigidbody>();

    private void FixedUpdate()
    {
        if (!IsServer) { return; }
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
        if (!other.attachedRigidbody.TryGetComponent<Rigidbody>(out Rigidbody player)) { return; }
        {
            playersInZone.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        if (!other.attachedRigidbody.TryGetComponent<Rigidbody>(out Rigidbody player)) { return; }
        {
            playersInZone.Remove(player);
        }
    }

    void Attract(Rigidbody shipBodies) // this may need to change if other objects with rigid bodies are added in
    {
        Rigidbody shipToPull = shipBodies.GetComponent<Rigidbody>();

        Vector3 direction = shipToPull.position - shipBodies.position; // figure out direction the ship is in 
        float distance = direction.sqrMagnitude;

        float pullForceMagnitude = (portalBody.mass * shipToPull.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * pullForceMagnitude;

        shipToPull.AddForce(force);
    }

}
