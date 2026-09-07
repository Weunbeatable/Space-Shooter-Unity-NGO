using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MinimapFollow : NetworkBehaviour
{
    public Rigidbody player;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        Vector3 newpPos = player.position;
        newpPos.y = transform.position.y;
        transform.position = newpPos;

        transform.rotation = Quaternion.Euler(90f, player.rotation.y, 0f);

    }
}
