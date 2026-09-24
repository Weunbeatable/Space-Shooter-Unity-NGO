using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag =="Field" || other.gameObject.tag == "Portal") { return; }
        Destroy(gameObject);
    }
}
