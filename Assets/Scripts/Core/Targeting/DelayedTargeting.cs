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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        
        inputReader.UseAbilityEvent += AttemptToUseAbility;
    }

    private void AttemptToUseAbility(bool obj)
    {
        hasTargettingStarted = !hasTargettingStarted;
        StartTargeting(playerReference);
    }

    public override void StartTargeting(GameObject user)
    {
        if (!IsOwner) { return; }

        ShipPlayer player = user.GetComponent<ShipPlayer>();
        player.StartCoroutine(Targeting(user, player));
    }

 

    private IEnumerator Targeting(GameObject user, ShipPlayer shipPlayer)
    {
        DisablePlayerControls();
        shipPlayer.enabled = false; // placeholder, actually need to disable movement and firing and spin
        while (true)
        {
            Debug.Log("looking for target");
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto); // change cursor during firing
            if(hasTargettingStarted == false)
            {
                EnablePlayerControls();
                yield break;
            }
            yield return null; // wait till next frame
        }

    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.UseAbilityEvent -= AttemptToUseAbility;
    }
    private void EnablePlayerControls()
    {
        //playerReference.GetComponent<PlayerMovement>().enabled = true;
        //playerReference.GetComponent<ThirdPersonControls>().enabled = true;
        //playerReference.GetComponent<PlayerAiming>().enabled = true;
        //playerReference.GetComponent<ProjectileLauncher>().enabled = true;
        //playerReference.GetComponent<Spin>().enabled = true;
    }
    private void DisablePlayerControls()
    {
        playerReference.GetComponent<PlayerMovement>().enabled = true;
        playerReference.GetComponent<ThirdPersonControls>().enabled = true;
        playerReference.GetComponent<PlayerAiming>().enabled = true;
        playerReference.GetComponent<ProjectileLauncher>().enabled = true;
        playerReference.GetComponent<Spin>().enabled = true;
    }
}
