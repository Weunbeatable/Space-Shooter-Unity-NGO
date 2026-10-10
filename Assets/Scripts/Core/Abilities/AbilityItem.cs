using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityItem : ScriptableObject
{
    public delegate void IAbilitiesDelegate(IAbilities ability);
    IAbilitiesDelegate delegateAbilityCallback;
    public enum InteractionType { tap, hold };

    protected bool isAbilityInUse;
    //protected Action onAbilityUseCompplete;
    [SerializeField] protected string abilityID;
    [SerializeField] protected string abilityName;
    [SerializeField][TextArea] protected string abilityDescription;
    [SerializeField] protected Sprite abilityIcon;
    [SerializeField] protected int coinCost; 

    public virtual void UseAbility()
    {
        Debug.Log("Ability In use");
    }

    public string GetAbilityName() => abilityName;
    public string GetAbilityDescription() => abilityDescription;

    public Sprite GetAbilityIcon() => abilityIcon;
}
