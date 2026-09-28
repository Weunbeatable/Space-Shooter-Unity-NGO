using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public abstract class Ability : NetworkBehaviour, IAbilities
{
    public delegate void IAbilitiesDelegate(IAbilities ability);
    IAbilitiesDelegate delegateAbilityCallback;
    public enum InteractionType { tap, hold};

    protected bool isAbilityInUse;
    protected Action onAbilityUseCompplete;
    [SerializeField] protected string abilityName;
    [SerializeField] protected string abilityDescription;
    [SerializeField] protected Sprite abilityIcon;

    public void UseAbility()
    {
        Debug.Log("Ability In use");
    }

    public string GetAbilityName() => abilityName;
    public string GetAbilityDescription() => abilityDescription;
}
