using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : NetworkBehaviour
{
    public static Action OnThirdPersonEnter; //  subscribed to by player movement, player aiming AND a player camera as those are separate concerns 
    public static Action OnTopDownEnter;

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] public Rigidbody playerBody;
    [SerializeField] private FlightCameraSettings topDownSettings;
    [SerializeField] private FlightCameraSettings thirdPersonSettings;
    
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 30f;
    [SerializeField] private bool didAltitudeChange;
    [Tooltip("Possible altitude changes the ship can make in the y axis. ")]
    [SerializeField] private float[] altitudePositions = { 1.0f, -30f };
    [Tooltip("Adjust this property to change how fast the ship will change altitude")]
    [SerializeField] private float altitudespeedChangeRate = .15f; // subject to change 
    [Tooltip("Smoothing of movement when switching velocities")]
    [SerializeField, Min(0.01f)] private float altitudeSmoothTime = 0.5f;
    private float altitudeVelocity;

    [SerializeField] private bool UseControls = true;
    [SerializeField] float spinDuration = 0.6f;
    [SerializeField] float spinAmmount = 360f;
    private bool isSpinning;

    Vector3 previousMovementInput;
    

    // network spwan instead of start to avoid sync issues.   
    public override void OnNetworkSpawn()
    {
      if(!IsOwner) return;
     
        inputReader.MoveEvent += HandleMove;
        inputReader.ChangeAltitudeEvent += ChangeAltitude;
        inputReader.SpinEvent += ActivateSpin;
    }

    public override void OnNetworkDespawn()
    {
       if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMove;
        inputReader.ChangeAltitudeEvent -= ChangeAltitude;
        inputReader.SpinEvent -= ActivateSpin;
    }

  

    void Update()
    {
        if (!IsOwner) return;
        // input of our device * -turn rate so its not inverted and make framerate independent
        if(UseControls == false) { return; }
        float yRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        
        bodyTransform.Rotate(0f, yRotation, 0f);
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        
        if(UseControls == false) { return; }
        playerBody.velocity = bodyTransform.forward.normalized* previousMovementInput.y * movementSpeed;

        float targetY = didAltitudeChange ? altitudePositions[1]: altitudePositions[0];
        Vector3 position = playerBody.position;
        position.y = Mathf.SmoothDamp(
            position.y,
            targetY, 
            ref altitudeVelocity,
            altitudeSmoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime);
       
        // finish at specified height when motion is done 
        if(Mathf.Abs(position.y - targetY) < 0.01f &&
            Mathf.Abs(altitudeVelocity) < 0.05f)
        {
            position.y = targetY;
            altitudeVelocity = 0f;
        }
        playerBody.position = position;
    }
    private void HandleMove(Vector2 movementInput)
    {
      //  previousMovementInput = movementInput;
      if(UseControls == false) { return; }
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }

    private void ChangeAltitude(bool altitude_Change)
    {
        // check if user presses button to switch altitude (aka position in the y axis) 
        Debug.Log("Updating altitude");
        didAltitudeChange = !didAltitudeChange;
    }

    public void TriggerThirdPersonMode()
    {
        if (!IsServer) { return; }

        // setup client rpc params
        ClientRpcParams clientRpc = new ClientRpcParams
        {
            // send params to targeted client in this case i'm getting the target based off thier Id
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]
                {
                    OwnerClientId,
                }
            }
        };
        ActivateThirdPersonControlsClientRpc(clientRpc);
        
    }
    internal void TriggerTopDownMode()
    {
        if (!IsServer) { return; }

        ClientRpcParams clientRpc = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]
                {
                    OwnerClientId,
                }
            }
        };
        ActivateTopDownControlsClientRpc(clientRpc);
        
    }

    [ClientRpc]
    private void ActivateThirdPersonControlsClientRpc(ClientRpcParams ship = default)
    {
        //disable controls
        UseControls = false;

        // Notify this client's camera + ThirdPersonControls
        OnThirdPersonEnter?.Invoke();

    }
    [ClientRpc]
    private void ActivateTopDownControlsClientRpc(ClientRpcParams ship = default)
    {
        UseControls = true;
        FlyBackToStartingTopDownHeight();
        OnTopDownEnter?.Invoke();
    }

    private void FlyBackToStartingTopDownHeight()
    {
        Vector3 newAltitude = playerBody.position;
        transform.rotation = Quaternion.identity;
        float ypos = playerBody.velocity.y;
        newAltitude.y = altitudePositions[0];
        Vector3 resetPosotion = playerBody.position;
        resetPosotion.y = altitudePositions[0];
        playerBody.position = resetPosotion;
    }

    private void ActivateSpin()
    {
         if(!UseControls || isSpinning) { return; } 
         
        if (UseControls == true && !DOTween.IsTweening(playerBody))
        {
            isSpinning = true;
            bodyTransform.DOLocalRotate(new Vector3(
                0f,
                0f,
                spinAmmount ), spinDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine).OnComplete(() => isSpinning = false);
        }
    }
}
