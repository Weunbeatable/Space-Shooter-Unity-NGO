using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pickup : NetworkBehaviour
{
    [SerializeField] Ability abilityTopickup;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.attachedRigidbody.TryGetComponent<AbilitySlot>(out AbilitySlot playerAbility)) { return; }
        {
            playerAbility.SetAbility(abilityTopickup);
            Debug.Log("Object acquired");
        }

        Destroy(this.gameObject);
    }
}
