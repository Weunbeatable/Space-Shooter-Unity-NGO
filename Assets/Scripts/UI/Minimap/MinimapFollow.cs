using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MinimapFollow : NetworkBehaviour
{
    [SerializeField] public Rigidbody player;
    [SerializeField] private Camera minimapCam;

    public override void OnNetworkSpawn()
    {
        // each device has every network player 
        // only the devices own player has the minimap camera enabled
        minimapCam.enabled = IsOwner; // only show the minimap camera if you are the owner of that specific map.
    }
    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        Vector3 newpPos = player.position;
        newpPos.y = transform.position.y;
        transform.position = newpPos;

        transform.rotation = Quaternion.Euler(90f, player.rotation.eulerAngles.y, 0f);

    }
}
