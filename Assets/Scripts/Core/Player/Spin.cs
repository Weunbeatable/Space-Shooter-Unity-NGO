using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class Spin : NetworkBehaviour
{
    [SerializeField] InputReader input;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        input.SpinEvent += ActivateSpin;
    }

    public override void OnNetworkDespawn()
    {
        input.SpinEvent -= ActivateSpin;
    }

    private void ActivateSpin()
    {
        if (!IsOwner) { return; }

       // this.gameObject.GetComponent<PlayerCameraSettings>();
    }
}
