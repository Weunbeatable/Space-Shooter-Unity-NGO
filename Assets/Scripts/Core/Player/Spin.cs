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
    [SerializeField] CapsuleCollider spinCollider;
    [SerializeField] ParticleSystem spinParticles;
    [SerializeField] AudioSource spinAudio;

    private float activationDuration;
    [Range(0f, 1f)]
    private float clipEffectRange;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        input.SpinEvent += ActivateSpin;
    }

    private void Update()
    {
        if (activationDuration <= 0)
        {
            spinCollider.gameObject.SetActive(false);
            return;
        }
        activationDuration -= Time.deltaTime;
    }

    public override void OnNetworkDespawn()
    {
        input.SpinEvent -= ActivateSpin;
    }

    private void ActivateSpin()
    {
        if (!IsOwner) { return; }
        spinAudio.pitch = UnityEngine.Random.Range(0.5f, 1f);
        spinAudio.PlayOneShot(spinAudio.clip);
        spinParticles.Play();
        spinCollider.gameObject.SetActive(true); 
        if(this.gameObject.TryGetComponent<ThirdPersonControls>(out ThirdPersonControls ship))
        {
            activationDuration = ship.GetSpinDuration();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner) { return; }
    }
}
