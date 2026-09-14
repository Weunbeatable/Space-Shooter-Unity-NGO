using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ThirdPersonControls : NetworkBehaviour
{
    [Header("input controller")]
    [Tooltip("The scriptable object for the game controls ")] [SerializeField] InputReader inputReader;
    [Header("General Setup Settings")]
    [Tooltip("How fast ship moves up and down based upon player input")][SerializeField] float controlSpeed = 20f;
    [Tooltip("control the x constraints for movement")][SerializeField] float xRange = 5f; // the range our player can move left and right
    [Tooltip("Control y constraints for movement")][SerializeField] float yRange = 7f;

    [Header("Ship position based  Tuning")]
    [Tooltip("position control of ship")][SerializeField] float positionPitchFator = -2f; // how much we affect the change in ship position upon using a control
    [Tooltip("adjust values to affect ships yaw position")][SerializeField] float positionYawFactor = -5f;
    [Tooltip("refernce to player rigidbody")][SerializeField] Rigidbody playerBody;
    [SerializeField] float ThrustValue = 10f;
    [SerializeField] float BrakeValue = 15f;

    [Header("pitch, yaw, and roll control aka X,Y,Z controls")]
    [Tooltip("Adjust to affect ships yaw movement")][SerializeField] float controlYawFactor = -10f;
    [Tooltip("adjust to controll the roll 'Z' factor of ship")][SerializeField] float controlRollFactor = -25f;
    [Tooltip("pitch control of ship")][SerializeField] float controlPitchFactor = -10f; // how much we see the pitch being affected by our mouse movement
    [SerializeField] private float yawRate = 90f;
    [SerializeField] private float pitchRate = 60f;
    Vector3 previousMovementInput;
    float yThrow;
    float xThrow;
    
    [Header("Controls activation status")]
    [Tooltip("Value that helps determine if current control scheme is being used")][SerializeField] private bool UseControls;
    [Tooltip("Value that determines if user is intending to move with forward momentum.")][SerializeField] private bool IsBoosting;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
        inputReader.BoostFireEvent += BoostForward;
        PlayerMovement.OnThirdPersonEnter += ActivateControls;
        PlayerMovement.OnTopDownEnter += DeactivateControls;
    }

   

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= HandleMove;
        inputReader.BoostFireEvent -= BoostForward;
        PlayerMovement.OnThirdPersonEnter -= ActivateControls;
        PlayerMovement.OnTopDownEnter -= DeactivateControls;
    }
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if(UseControls == false) return;
        ProcessTranslation();
        ProcessRotation();
    }

    private void ProcessRotation()
    {
        float yaw = previousMovementInput.x * yawRate * Time.fixedDeltaTime;
        float pitch = -previousMovementInput.y * pitchRate * Time.fixedDeltaTime;

        Quaternion rotationDelta = Quaternion.Euler(pitch, yaw, 0f);

        playerBody.MoveRotation(playerBody.rotation * rotationDelta);
    }

    private void ProcessTranslation()
    {

        float xOffset = previousMovementInput.x * Time.deltaTime * controlSpeed;
        float yOffset = previousMovementInput.y * Time.deltaTime * controlSpeed;


        float rawXPos = playerBody.position.x + xOffset;
        float rawYPos = playerBody.position.y + yOffset;

        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange); // this allows us to specify how far left and right our ship can move. 
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange + 5, yRange);

       // playerBody.position = new Vector3(clampedXPos, clampedYPos, playerBody.position.z);// this allows us to move the ship in 3D space hence vector3, clampedxpos restricts our left and right movement
        if(IsBoosting == true)
        {
            //playerBody.AddForce(transform.forward * ThrustValue, ForceMode.Force);
            Vector3 shipComponent = transform.forward.normalized;
            //shipComponent.y = Mathf.Clamp(shipComponent.y, -30f, 5f);
            //shipComponent.x = 0f;
            playerBody.velocity = -shipComponent.normalized  *ThrustValue;
        }
        else
        {
            if(playerBody.velocity.magnitude > 0.1f) // as long as i'm still moving add a break force
            {
                Vector3 brakeForce = -playerBody.velocity.normalized;
                playerBody.AddForce(brakeForce * BrakeValue, ForceMode.Force);
            }
        }

        Debug.Log("X_Controller_Pos " + clampedXPos);
        Debug.Log("Y_Controller_Pos " + clampedYPos);
    }

    private void HandleMove(Vector2 movementInput)
    {
        //  previousMovementInput = movementInput;
        if (UseControls == false) { return; }
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }

    private void ActivateControls()
    {
        UseControls = true;
    }
    private void DeactivateControls()
    {
        UseControls = false;
    }
    private void BoostForward(bool IsBoosting)
    {
        this.IsBoosting = IsBoosting;
    }

}
