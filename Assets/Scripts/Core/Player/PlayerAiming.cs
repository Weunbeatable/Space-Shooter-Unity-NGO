using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
   [SerializeField] private InputReader inputReader;

   [SerializeField] private Transform turretTransform;
    // check each frame where mouse is and relative to where turret is to rotate turret to cursor
    // late update will allow ship to move first then turret logic will happen

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

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

            if (aimDirection.sqrMagnitude >Mathf.Epsilon)
            {
                turretTransform.forward =
                    aimDirection.normalized;
            }
        }
    }
}
