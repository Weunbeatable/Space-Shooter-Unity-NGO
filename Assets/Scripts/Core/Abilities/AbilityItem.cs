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
    // make the id property readonly to prevent accidental writing
    public string AbilityID => abilityID;
    [SerializeField] protected string abilityName;
    [SerializeField][TextArea] protected string abilityDescription;
    [SerializeField] protected Sprite abilityIcon;
    [SerializeField] protected int coinCost;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(abilityID))
        {
            // If i forgot to give the ability ann ID when setting it up, just generate a new Unique ID for it
            abilityID = System.Guid.NewGuid().ToString("N");
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
    public virtual void UseAbility()
    {
        Debug.Log("Ability In use");
    }

    public string GetAbilityName() => abilityName;
    public string GetAbilityDescription() => abilityDescription;

    public Sprite GetAbilityIcon() => abilityIcon;
}
