using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedTargeting : TargetingStrategy
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 cursorHotspot;
    [SerializeField] private InputReader inputReader;
    public override void StartTargeting(GameObject user)
    {
        if (!IsOwner) { return; }
        ShipPlayer player = user.GetComponent<ShipPlayer>();
        player.StartCoroutine(Targeting(user, player));
    }

    private IEnumerator Targeting(GameObject user, ShipPlayer shipPlayer)
    {
        shipPlayer.enabled = false; // placeholder, actually need to disable movement and firing and spin
        while (true)
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto); // change cursor during firing
            if(shipPlayer.inpu)
            yield return null; // wait till next frame
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        w
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
    }
    private void EnablePlayerControls()
    {
        //playerReference.GetComponent<PlayerMovement>().enabled = true;
        //playerReference.GetComponent<ThirdPersonControls>().enabled = true;
        //playerReference.GetComponent<PlayerAiming>().enabled = true;
        //playerReference.GetComponent<ProjectileLauncher>().enabled = true;
        //playerReference.GetComponent<Spin>().enabled = true;
    }
}
