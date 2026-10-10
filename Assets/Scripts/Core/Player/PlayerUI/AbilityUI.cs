using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : NetworkBehaviour
{
    [SerializeField] AbilitySlot slotReference;
    [SerializeField] Image abilityImage;
    [SerializeField] TextMeshProUGUI abilityText;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        slotReference.AbilityEquipStatusChange += UpdateUI;
    }

    private void UpdateUI(Ability ability)
    {
       if(ability == null)
        {
            abilityImage.sprite = null;
        }

        abilityImage.sprite = ability.GetAbilityIcon();
        abilityText.text = ability.GetAbilityName();
    }

    public override void OnNetworkDespawn()
    {
        slotReference.AbilityEquipStatusChange -= UpdateUI;
    }
}
