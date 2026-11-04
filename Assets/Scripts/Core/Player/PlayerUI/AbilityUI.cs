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
    [SerializeField] TextMeshProUGUI abilityDescription;


    private void OnValidate()
    {
        // make sure no references are missing 
        if(slotReference == null)
        {
            Debug.LogWarning("AbilityUI: Ability slot reference is missing!", this);
        }

        // image reference check
        if (abilityImage == null)
        {
            Debug.LogWarning("AbilityUI: Ability Image reference is missing!", this);

        }
        // ability text reference check
        if (abilityText == null)
        {
            Debug.LogWarning("AbilityUI: Ability Name reference is missing!", this);
        }
        // ability description reference check
        if (abilityDescription == null)
        {
            Debug.LogWarning("AbilityUI: Ability description reference is missing!", this);
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        Color updateColor = abilityImage.color;
        updateColor.a = 0.0f;
        abilityImage.color = updateColor;
        abilityText.text = string.Empty;
        slotReference.AbilityEquipStatusChange += UpdateUI;
    }

    private void UpdateUI(Ability ability)
    {
        if (!IsOwner) { return; }
        Debug.Log($"UI received ability: {(ability == null ? "NULL" : ability.name)}");
        if (ability == null)
        {
            Color updateColor = abilityImage.color;
            updateColor.a = 0.0f;
            abilityImage.color = updateColor;
            abilityImage.sprite = null;
            abilityText.text = string.Empty;
            return;
        }

        abilityImage.sprite = ability.GetAbilityIcon();
        Color FullColor = abilityImage.color;
        FullColor.a = 1.0f;
        abilityImage.color = FullColor;
        abilityText.text = ability.GetAbilityName();

        Debug.Log("Skill slot now contains " + ability.name);
    }

    public override void OnNetworkDespawn()
    {
        slotReference.AbilityEquipStatusChange -= UpdateUI;
    }
}
