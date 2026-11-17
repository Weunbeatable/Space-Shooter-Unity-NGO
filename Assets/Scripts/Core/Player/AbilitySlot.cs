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
    private void ActivatingSkill(bool wasTriggerFired)
    {
        // if skills are based on coin cost there should be an attempt to activate the coin cost can be checked initally here in ability slot but for Use over time UOT it can also be checked in the ability itself. 
        // may need to modify later but if the strategy type doesn't have a delay on it based on some state change then use regular skill, this is intended to prevent two intance of Use firing at once.
        //if(skill.targetingStrategy is not DelayedTargeting)
        //{
        //    skill.Use(this.gameObject);
        //}
        //else
        //{
        //    skill.Use(this.gameObject, wasTriggerFired);
        //}
        Debug.Log("starting targeting");
        skill.Use(this.gameObject, wasTriggerFired);

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
