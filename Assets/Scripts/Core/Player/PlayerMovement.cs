using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody playerBody;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    Vector3 previousMovementInput;
    // network spwan instead of start to avoid sync issues.   
    public override void OnNetworkSpawn()
    {
      if(!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
    }


    public override void OnNetworkDespawn()
    {
       if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMove;
    }
    void Update()
    {
        if (!IsOwner) return;
        // input of our device * -turn rate so its not inverted and make framerate independent
        float yRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        
        bodyTransform.Rotate(0f, yRotation, 0f);
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        
        playerBody.velocity = bodyTransform.forward.normalized* previousMovementInput.y * movementSpeed;
    }
    private void HandleMove(Vector2 movementInput)
    {
      //  previousMovementInput = movementInput;
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }
}
