using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionItem : MonoBehaviour
{
public virtual void Use(GameObject user)
    {
        Debug.Log("Using ability" + this);
    }
}
