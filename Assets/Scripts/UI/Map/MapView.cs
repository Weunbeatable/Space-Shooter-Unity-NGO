using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MapView : NetworkBehaviour
{
    [SerializeField] InputReader _inputReader;
    [SerializeField] GameObject Map;
    [SerializeField] GameObject fullMapIcon;
    public override void OnNetworkSpawn()
    {
        _inputReader.MapViewEvent += ToggleMap;
        fullMapIcon.SetActive(IsOwner);
    }

    public override void OnNetworkDespawn()
    {
        _inputReader.MapViewEvent -= ToggleMap;
    }

    private void ToggleMap(bool toggleState)
    {
        if (!IsOwner) { return; }
        if(toggleState == true)
        {
            Map.SetActive(true);
            
        }
        else
        {
          Map.SetActive(false); 
        }
    }
}
