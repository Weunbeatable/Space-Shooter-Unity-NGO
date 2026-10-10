using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pickup : NetworkBehaviour
{
    [SerializeField] Ability abilityTopickup;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.TryGetComponent<AbilitySlot>(out AbilitySlot playerAbility)) { return; }
        {
            playerAbility.SetAbility(abilityTopickup);
        }

        Destroy(this.gameObject);
    }
}
