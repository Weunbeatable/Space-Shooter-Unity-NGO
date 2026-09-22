using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
   [SerializeField] private InputReader inputReader;

   [SerializeField] private Transform turretTransform;
    [SerializeField] private Transform turretMount;
    // check each frame where mouse is and relative to where turret is to rotate turret to cursor
    // late update will allow ship to move first then turret logic will happen
    Vector3 newRotation;
    float xVel;
    float yVel;
    float zVel;
    float smoothTime = 0.5f;
    private bool useThirdPersonAiming = false;
    public bool IsUsingGamepad;
    private bool wasUsingGamepad;
    private Camera mainCamera;

    [Header("Third Person Aim")]
    [SerializeField] private float thirdPersonAimDistance = 100f;
    [SerializeField] private float turretAimSpeed = 360f;

    [SerializeField] private float maxYaw = 90f;
    [SerializeField] private float maxPitch = 25f;

    [Header("Controller values")]
    private Vector2 controllerAim;
    [SerializeField] private float controllerAimSpeed = 1.5f; // will want to expose this in the menu for adjustment

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        PlayerMovement.OnTopDownEnter += UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter += UseThirdPersonAiming;
    }

  

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        PlayerMovement.OnTopDownEnter -= UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter -= UseThirdPersonAiming;
    }
    private void LateUpdate()
    {
        if (!IsOwner)
            return;
        bool usingGamepad = inputReader.IsUsingGamepad;
        if(usingGamepad && !wasUsingGamepad)
        {
            SyncControllerAimToMouse();
        }
        wasUsingGamepad = usingGamepad;
        // ensure right camera is grabbed
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null) return;
        }
        if (useThirdPersonAiming == false)
        {
            if (inputReader.IsUsingGamepad)
            {
                TopDownControllerAim(inputReader.ControllerAim);
            }
            else
            {
                TopDownMouseControls();
            }

        }
        else
        {
            {

                if (inputReader.IsUsingGamepad)
                {
                    ThirdPersonAimingController(inputReader.ControllerAim);
                }
                else
                {
                    ThirdPersonAimControlMouse();
                }
            }
        }
    }
    private void applyNormalizedThirdPersonAim(Vector2 normalizedAim)
    {
        float yaw = normalizedAim.x * maxYaw;

        float pitch = -normalizedAim.y * maxPitch;
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        turretTransform.localRotation = Quaternion.RotateTowards(turretTransform.localRotation, targetRotation, turretAimSpeed * Time.deltaTime);
    }
    private void ThirdPersonAimControlMouse()
    {
        /*
         * goal
         * center of screen = turret faces straight ahead,
            left edge = about -90° yaw,
            right edge = about +90° yaw,
            upper/lower screen = controlled pitch,
            turret never aims behind the ship.
        */
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


    private void TopDownMouseControls()
    {
        Vector2 mousePosition = inputReader.AimPosition;

        Ray aimRay = mainCamera.ScreenPointToRay(mousePosition);

        applyTopDownRay(aimRay);
    }
    private void TopDownControllerAim(Vector2 stickInput)
    {
        UpdateControllerAim(stickInput);

        Vector2 viewPort = ControllerAimToViewport();

        Ray aimRay = mainCamera.ViewportPointToRay(new Vector3(viewPort.x, viewPort.y, 0f));

        applyTopDownRay(aimRay);
    }

    private void applyTopDownRay(Ray aimRay)
    {
        // same logic for the mouse for creating a ray to allow for proper aiming but reapplied to work for controller as well
        Plane groundPlane =
            new Plane(Vector3.up, turretTransform.position);

        if (!groundPlane.Raycast(aimRay, out float distance)) { return; }

        Vector3 targetPosition = aimRay.GetPoint(distance);

        Vector3 aimDirection = targetPosition - turretTransform.position;
        aimDirection.y = 0f;

        if(aimDirection.sqrMagnitude > Mathf.Epsilon)
        {
            // some input was registered so set the forward of the turret
            turretTransform.forward = aimDirection.normalized;
        }
    }

    private Vector2 ControllerAimToViewport()
    {
        // topdown conversion for viewport coordinates
        return new Vector2((controllerAim.x + 1f) * 0.5f,
            (controllerAim.y + 1f) * 0.5f);
    }

    // prevent aim change feeling drastic if changing inputs 
    private void SyncControllerAimToMouse()
    {
        Vector2 viewport = mainCamera.ScreenToViewportPoint(inputReader.AimPosition);
        controllerAim = new Vector2(
            (viewport.x - 0.5f) * 2f,
            (viewport.y - 0.5f) * 2f);
    }

    private void UseTopDownAiming()
    {
        useThirdPersonAiming = false;
    }

    private void UseThirdPersonAiming()
    {
        useThirdPersonAiming = true; 
    }

    /* virtual mouse for contorller visual 
 * (-1,+1) -------- (+1,+1)
    |                |
    |      (0,0)     |
    |                |
(-1,-1) -------- (+1,-1)

               INPUT
                      │
             ┌────────┴────────┐
             │                 │
           Mouse           Controller
             │                 │
      absolute pixels       stick delta
             │                 │
             │          virtual cursor
             │                 │
             └────────┬────────┘
                      ↓
                 normalized aim
                      │
          ┌───────────┴───────────┐
          ↓                       ↓
   Third-person aim          Top-down ray
   yaw / pitch              world position
*/
}
