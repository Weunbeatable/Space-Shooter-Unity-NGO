using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class DirectionalTargeting : TargetingStrategy
{
    public override void StartTargeting(GameObject user)
    {
        print("looking At Direction to Target");
    }

    public override void StartTargeting(GameObject user, bool statehasSwitched)
    {
        throw new System.NotImplementedException();
    }
}
