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
    [SerializeField] private bool didAltitudeChange;
    [Tooltip("Possible altitude changes the ship can make in the y axis. ")]
    [SerializeField] private float[] altitudePositions = { 1.0f, -30f };
    [Tooltip("Adjust this property to change how fast the ship will change altitude")]
    [SerializeField] private float altitudespeedChangeRate = .15f; // subject to change 

    Vector3 previousMovementInput;

    // network spwan instead of start to avoid sync issues.   
    public override void OnNetworkSpawn()
    {
      if(!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
        inputReader.ChangeAltitudeEvent += ChangeAltitude;
    }

   

    public override void OnNetworkDespawn()
    {
       if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMove;
        inputReader.ChangeAltitudeEvent -= ChangeAltitude;
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

        Vector3 newAltitude = playerBody.position;
        float ypos = playerBody.velocity.y;
        if (!didAltitudeChange)
        {
            newAltitude.y = altitudePositions[0];
            playerBody.position = Vector3.Lerp(playerBody.position, newAltitude, altitudespeedChangeRate * Time.deltaTime);
        }
        else
        {
            newAltitude.y = altitudePositions[1];
            playerBody.position = Vector3.Lerp(playerBody.position, newAltitude, altitudespeedChangeRate * Time.deltaTime);
        } 
    }
    private void HandleMove(Vector2 movementInput)
    {
      //  previousMovementInput = movementInput;
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }

    private void ChangeAltitude(bool altitude_Change)
    {
        // check if user presses button to switch altitude (aka position in the y axis) 
        Debug.Log("Updating altitude");
        didAltitudeChange = !didAltitudeChange;
    }
}
