using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitySlot : NetworkBehaviour
{ 
    [SerializeField] public Ability skill;
    [SerializeField] bool isChargeable;
    [SerializeField] InputReader inputReader;

    public delegate void OnAbiliityEquipped(Ability ability);
    public event OnAbiliityEquipped AbilityEquipStatusChange;
    public override void OnNetworkSpawn()
    {
       if(!IsOwner) { return; }
        inputReader.RemoveSkillEvent += SkillCleanup;
        inputReader.UseAbilityEvent += ActivatingSkill;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.RemoveSkillEvent -= SkillCleanup;
        inputReader.UseAbilityEvent -= ActivatingSkill;
    }
    private void ActivatingSkill(bool obj)
    {
        // if skills are based on coin cost there should be an attempt to activate the coin cost can be checked initally here in ability slot but for Use over time UOT it can also be checked in the ability itself. 
        throw new NotImplementedException();
    }
    public void SetAbility(Ability abilitySet)
    {
        skill = abilitySet;
        Debug.Log("Skill slot now contains " + skill.name);
        AbilityEquipStatusChange?.Invoke(skill);
    }

    public void ClearAbility()
    {
        SetAbility(null);
    }

    private void SkillCleanup()
    {
        if(skill == null) { return; }
        ClearAbility();
    }
}
