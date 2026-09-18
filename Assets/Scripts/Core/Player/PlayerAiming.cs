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

        // solution from joah wong in unity multiplayer netcode course ty chat

        Vector3 mouseScreenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            -Camera.main.transform.position.z);
        Vector3 aimWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        //aimWorldPos.z = 0f; // don't need depth;

        Vector3 direction = (aimWorldPos - turretMount.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        //newRotation = new Vector3(
        //    Mathf.SmoothDamp(newRotation.x, targetRotation.x, ref xVel, smoothTime * Time.fixedDeltaTime),
        //    Mathf.SmoothDamp(newRotation.y, targetRotation.y, ref yVel, smoothTime * Time.fixedDeltaTime),
        //    Mathf.SmoothDamp(newRotation.z, targetRotation.z, ref zVel, smoothTime * Time.fixedDeltaTime));

        //turretTransform.eulerAngles = newRotation;
        turretTransform.localRotation = Quaternion.RotateTowards(turretTransform.localRotation, targetRotation, 180f * Time.fixedDeltaTime);
        Debug.Log("local rotation is " + turretTransform.localRotation);
        Debug.Log("target rotation is " + targetRotation);
        // turretMount.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 1000 );
       // Debug.Log("TARGET ROTATION IS " + newRotation);

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
