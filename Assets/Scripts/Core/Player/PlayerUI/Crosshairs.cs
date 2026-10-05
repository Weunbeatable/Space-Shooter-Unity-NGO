using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Crosshairs : NetworkBehaviour
{
    [SerializeField] Image centerDotIcon;
    [SerializeField] Image crossHairsIcon;
    [SerializeField] InputReader inputReader;
    [Tooltip("Set aim mask for things the crosshair should stop at e.g. damagable objects other ships environment etc.")]
    [SerializeField] LayerMask aimMask;
    [SerializeField] float maxAimDistance = 100f;
    [SerializeField] Vector3 initialScale;
    [SerializeField] Vector3 firingScale;
    [SerializeField] Color initialColor;
    [SerializeField] Color centerDotFiringColor;
    [SerializeField] Color crossHairFiringColor;
    [Header("references")]
    private Camera mainCamera;
    [SerializeField] ProjectileLauncher projecileLauncherReference;

    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        initialScale = crossHairsIcon.rectTransform.localScale;
        //may remove this so that its only changed in inspector
        firingScale = new Vector3(0.3f, 0.3f, 0.3f);
        initialColor = Color.white;
        centerDotFiringColor = Color.red;
        crossHairFiringColor = new Color(150, 119, 119);
        crossHairsIcon.gameObject.SetActive(false);
        centerDotIcon.gameObject.SetActive(false);
        PlayerMovement.OnThirdPersonEnter += UseThirdPersonAiming;
        PlayerMovement.OnTopDownEnter += UseTopDownAiming;
        inputReader.PrimaryFireEvent += TriggerDynamicCrosshair;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        PlayerMovement.OnThirdPersonEnter -= UseThirdPersonAiming;
        PlayerMovement.OnTopDownEnter -= UseTopDownAiming;
        inputReader.PrimaryFireEvent += TriggerDynamicCrosshair;
    }
    private void Update()
    {
        if (!IsOwner) { return; }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null) return;
        }

        centerDotIcon.rectTransform.position = inputReader.AimPosition;
        Vector3 origin = projecileLauncherReference.GetProjectileSpawnPoint().position;
        Vector3 direction = projecileLauncherReference.GetProjectileSpawnPoint().forward; // so it matches the projectile launcher
        Vector3 aimPoint = origin + direction * maxAimDistance;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
        {
            // send out a raycast from the projecitle launcher spawn point in the direction of the 
            // launchers forwards direction so it lines up correctly at all times. Then have the point its aiming at be further
            // out so the origin/starting point + direction and multiplied by the aim distance to indicate max diistance it should
            // aim towards. 
            aimPoint = hit.point;
        }

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(aimPoint);
        centerDotIcon.enabled = screenPoint.z > 0f;

        if (centerDotIcon.enabled)
        {
            centerDotIcon.rectTransform.position = screenPoint;
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner || !crossHairsIcon.gameObject.activeSelf) { return; }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null) return;
        }

        crossHairsIcon.rectTransform.position = inputReader.AimPosition;

        Vector3 origin = projecileLauncherReference.GetProjectileSpawnPoint().position;
        Vector3 direction = projecileLauncherReference.GetProjectileSpawnPoint().forward; // so it matches the projectile launcher
        Vector3 aimPoint = origin + direction * maxAimDistance; 

      if(Physics.Raycast(origin, direction, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
        {
            // send out a raycast from the projecitle launcher spawn point in the direction of the 
            // launchers forwards direction so it lines up correctly at all times. Then have the point its aiming at be further
            // out so the origin/starting point + direction and multiplied by the aim distance to indicate max diistance it should
            // aim towards. 
            aimPoint = hit.point; 
        }

      Vector3 screenPoint = mainCamera.WorldToScreenPoint(aimPoint);
        crossHairsIcon.enabled = screenPoint.z > 0f;

        if (crossHairsIcon.enabled)
        {
            crossHairsIcon.rectTransform.position = screenPoint;
        }
    }

    private void UseThirdPersonAiming()
    {
        crossHairsIcon.gameObject.SetActive(true);
        centerDotIcon.gameObject.SetActive(true);
    }

    private void UseTopDownAiming()
    {
        crossHairsIcon.gameObject.SetActive(false);
        centerDotIcon.gameObject.SetActive(false);
    }

    private void TriggerDynamicCrosshair(bool isFiring)
    {
        if(!IsOwner) return;
        if(isFiring == true)
        {
            crossHairsIcon.rectTransform.DOScale(firingScale, 0.1f).SetEase(Ease.OutSine);
            crossHairsIcon.color = crossHairFiringColor;
            centerDotIcon.color = centerDotFiringColor;
        }
        if (isFiring == false && crossHairsIcon.rectTransform.localScale != initialScale)
        {
            crossHairsIcon.rectTransform.DOScale(initialScale, 0.15f).SetEase(Ease.InOutSine);
            crossHairsIcon.color = initialColor;
            centerDotIcon.color = initialColor;
        }
    }
}
