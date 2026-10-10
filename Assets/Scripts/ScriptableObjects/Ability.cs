using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Ship_Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ActionItem
{
    [SerializeField] TargetingStrategy targetingStrategy;
    public override void Use(GameObject user)
    {
        targetingStrategy.StartTargeting(user);
    }
}
