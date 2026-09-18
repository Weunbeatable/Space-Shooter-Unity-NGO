using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class Teleport : NetworkBehaviour
{
    public static Action<ShipPlayer> OnTeleport;
    [SerializeField] private Transform TeleportExitPoint;
    public List<ShipPlayer> PlayersInTeleporter = new List<ShipPlayer>();
    
    private Vector3 SphereCenterPosition; 

    public override void OnNetworkSpawn()
    {
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    private void LateUpdate()
    {
        if (!IsServer) {  return; }
        //foreach(ShipPlayer player in PlayersInTeleporter)
        //{
        //    player.playerBody.position = TeleportExitPoint.position;
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        //if (!other.attachedRigidbody.TryGetComponent<ShipPlayer>(out ShipPlayer player)) {  return; }
        //{
        //    PlayersInTeleporter.Add(player);
        //}
        if (!other.attachedRigidbody.TryGetComponent<ShipPlayer>(out ShipPlayer player)) { return; }
        {
            player.RequestToTeleportClientShip(TeleportExitPoint.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) { return; }
        //if (!other.attachedRigidbody.TryGetComponent<ShipPlayer>(out ShipPlayer player)) { return ; }
        //{
        //    PlayersInTeleporter.Remove(player);
        //}
    }
}
