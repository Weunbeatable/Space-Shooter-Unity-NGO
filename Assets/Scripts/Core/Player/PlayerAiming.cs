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
    private Camera mainCamera;

    [Header("Third Person Aim")]
    [SerializeField] private float thirdPersonAimDistance = 100f;
    [SerializeField] private float turretAimSpeed = 360f;

    [SerializeField] private float maxYaw = 90f;
    [SerializeField] private float maxPitch = 45f;

    public override void OnNetworkSpawn()
    {
        PlayerMovement.OnTopDownEnter += UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter += UseThirdPersonAiming;
    }

  

    public override void OnNetworkDespawn()
    {
        PlayerMovement.OnTopDownEnter -= UseTopDownAiming;
        PlayerMovement.OnThirdPersonEnter -= UseThirdPersonAiming;
    }
    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        // ensure right camera is grabbed
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null) return;
        }
        if(useThirdPersonAiming == false)
        TopDownAimControls();

        if (useThirdPersonAiming == true)
        {
            ThirdPersonAimControl();
        }
    }

    private void ThirdPersonAimControl()
    {

        Vector2 mousePosition = inputReader.AimPosition;

        Ray mouseRay = mainCamera.ScreenPointToRay(mousePosition);

        // put a  plane in front of the ship and turret irregardless of wolrd rotation
        // this goes back to an earlier solution, the math was just wrong with the plane
        // corrective math was found in chat after other failed attempts that can be seen in past commits. 
        Vector3 planePosition = turretMount.position + turretMount.forward * thirdPersonAimDistance;

        Plane aimingPlane = new Plane(turretMount.forward, planePosition);

        if(!aimingPlane.Raycast(mouseRay, out float distance)) { return; } // dont calculate if ray doesn't hit anything. 

        Vector3 targetWorldPosition = mouseRay.GetPoint(distance); // figure out where the ray hits to find intersection 
        
        Vector3 worldDirection = (targetWorldPosition - turretMount.position).normalized; // figure out direction to face by subtracting and normalizing vectors

        // convert world direction to turrets local space. 
        Vector3 localDirection = turretMount.InverseTransformDirection(worldDirection);

        float yaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg; 

        float horizontalDistance = Mathf.Sqrt(localDirection.x * localDirection.x+localDirection.z * localDirection.z);

        float pitch = -Mathf.Atan2(localDirection.y, horizontalDistance) * Mathf.Rad2Deg; // - value to account for mouse input and facing direction of mount

        // lock turret facing to only front view
        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        Quaternion targetLocalRotation = Quaternion.Euler(-pitch, yaw, 0f);

        turretTransform.localRotation = Quaternion.RotateTowards(turretTransform.localRotation, targetLocalRotation, turretAimSpeed * Time.deltaTime);

    }

    private void TopDownAimControls()
    {
        Vector2 mousePosition = inputReader.AimPosition;

        Ray mouseRay =
            mainCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane =
            new Plane(Vector3.up, turretTransform.position);

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 targetPosition =
                mouseRay.GetPoint(distance);

            Vector3 aimDirection =
                targetPosition - turretTransform.position;

            aimDirection.y = 0f;

            if (aimDirection.sqrMagnitude > Mathf.Epsilon)
            {
                turretTransform.forward =
                    aimDirection.normalized;
            }
        }
    }

    private void UseTopDownAiming()
    {
        useThirdPersonAiming = false;
    }

    private void UseThirdPersonAiming()
    {
        useThirdPersonAiming = true; 
    }
}
