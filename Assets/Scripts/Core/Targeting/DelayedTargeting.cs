using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 based of the course you click a UI button to trigger the activation of a skill then you click again to set 
the position of targetting. Because the nature of skill usage is different (click  trigger on keybaord or gamepad)
That is what starts the targetting but clicking it again also triggers the position where the targetting ends
because of this The state needs to be tracked at all times and data passed in needs to be handled a little
differently. 
 */
public class DelayedTargeting : TargetingStrategy
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private bool hasTargettingStarted;
    [SerializeField] private GameObject playerReference;
    [SerializeField] private Transform cursorTarget;

    [Header("Controller values")]
    private Vector2 controllerAim;
    [SerializeField] private float controllerAimSpeed = 1.5f; // will want to expose this in the menu for adjustment

    private float smoothSpeed = 5.0f; 

    Vector3 previousMovementInput;
    private bool hasFinihedTargeting;
    private Camera mainCamera;
    private bool IsUsingGamepad;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        
      //  inputReader.UseAbilityEvent += AttemptToUseAbility;
        inputReader.PrimaryFireEvent += InputReader_PrimaryFireEvent;
    }



    private void LateUpdate()
    { if (!IsOwner) { return; }
        if(mainCamera == null)
        {
            mainCamera = Camera.main;

//if(mainCamera == null) return;
        }
    } 

     private void UpdateControllerAim(Vector2 controlStickInputs)
    {
        const float deadZone = 0.1f; // account for controller offset

        if (controlStickInputs.magnitude < deadZone)
        {
            controlStickInputs = Vector2.zero;
        }

        controllerAim += (controlStickInputs) * controllerAimSpeed * Time.deltaTime ;

        controllerAim.x = Mathf.Clamp(controllerAim.x, -1f, 1f); // set boundaries for how far left and right inputs can be read at
        controllerAim.y = Mathf.Clamp(controllerAim.y, -1f, 1f);
    }

    private void ThirdPersonAimingController(Vector2 stickInput)
    {
        UpdateControllerAim(stickInput);
        applyNormalizedThirdPersonAim(controllerAim);
    }
    private void applyNormalizedThirdPersonAim(Vector2 normalizedAim)
    {
        float xpos = normalizedAim.x ;

        float ypos = normalizedAim.y ;
        Vector3 targetPosition = new Vector3(xpos, ypos, 0f);

        cursorTarget.position = Vector2.Lerp(cursorTarget.position, targetPosition, smoothSpeed * Time.deltaTime);
   
    }
    private void ThirdPersonAimControlMouse()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
            Vector2 mousePosition = inputReader.AimPosition;

        Vector2 viewport =
         mainCamera.ScreenToViewportPoint(mousePosition);

        //convert 0 - 1 screen coordinates to -1 - 1
        // store it in a vector to pass to normalization function
        Vector2 normalizedAim = new Vector2(
        (viewport.x - 0.5f) * 2f,
        (viewport.y - 0.5f) * 2f);

        applyNormalizedThirdPersonAim(normalizedAim);

    }

    private void AttemptToUseAbility(bool obj)
    {
        // hasTargettingStarted = !hasTargettingStarted;
        hasTargettingStarted = true;
        StartTargeting(playerReference);
    }

    public override void StartTargeting(GameObject user)
    {
        //hasTargettingStarted = false;
        Debug.Log("Might go back to this version and scrap the overload since Fire button circumvents the needs for an overload");
    }
    private void InputReader_PrimaryFireEvent(bool obj)
    {
        if (!IsOwner) { return; }
        if (hasTargettingStarted == false) { return; } // no need to even try setting cursor target because targetting has not started, Access to this script should not have started but in some strange scenario it does don't trigger anything
        hasTargettingStarted = false;
    }
    public override void StartTargeting(GameObject user, bool statehasSwitched)
    {
        //if (!IsOwner) { return; }

        hasTargettingStarted = true;
        ShipPlayer player = user.GetComponent<ShipPlayer>();
        Debug.Log("Advanced targeting has started");
        player.StartCoroutine(Targeting(user, player));
    }

    //PROBLEM IS CALLING ABILITY BUTTON AGAIN WOULD FIRE THE COROUTINE AGAIN, IN THIS SCRIPT CAN JUST PROCESS INPUT READER SO WAIT FO RPLAYER TO PRESS FIRE BUTTON 
    private IEnumerator Targeting(GameObject user, ShipPlayer shipPlayer)
    {
        playerReference = user;
        if(user.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            Debug.Log("Sucessfully retrieved player contols");
            playerMovement.enabled = false;
        }
        while (hasTargettingStarted == true)
        {
            Debug.Log("looking for target");
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto); // change cursor during firing
            shipPlayer.AssignAbilityLookTarget(playerReference.transform,playerReference.transform);
            shipPlayer.SetAbilityPriority(19);

            //set cursor moving logic
            
            if (IsUsingGamepad)
                {
              ThirdPersonAimingController(inputReader.ControllerAim);
                }
            else
                {
             ThirdPersonAimControlMouse();
                }

            if(hasTargettingStarted == false)
            {
                if (user.TryGetComponent<PlayerMovement>(out PlayerMovement Movement))
                {
                    Debug.Log("Sucessfully retrieved player contols");
                    shipPlayer.AbiltyCameraPriorityReset();
                    shipPlayer.AssignAbilityLookTarget(null, null);
                    Movement.enabled = true;
                }
                yield break;
            }
            yield return null; // wait till next frame
        }

    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
       // inputReader.UseAbilityEvent -= AttemptToUseAbility;
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }
}
