using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // on client need to know if we own object or not 
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // assign ownership to whoever htey belong to
        CanCommitToTransform = IsOwner;
    }

    protected override void Update()
    {
        // makes sure we're still the owner each frame. 
        CanCommitToTransform = IsOwner;

        base.Update();
        //You dont have to commit transform if client is server.
        {
            CanCommitToTransform = IsOwner;
            base.Update();
            if (!IsHost && NetworkManager != null && NetworkManager.IsConnectedClient && CanCommitToTransform)
            {
                TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
            }
        }
    }
    // know on server if we have authority over it
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
