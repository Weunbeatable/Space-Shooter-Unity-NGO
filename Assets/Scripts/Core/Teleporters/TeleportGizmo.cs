using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // object will have an icon or mesh only visible in scene view to make it easily visible 
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, 10);
    }
}
