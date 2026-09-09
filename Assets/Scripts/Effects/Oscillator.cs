using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Oscillator : NetworkBehaviour
{
    [SerializeField] Rigidbody playerBody;
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [] InputReader inputReader;
    //todo remove from inspector later
    [Range(0, 1)] //  the modifying movement factor
    [SerializeField]
    float movementFactor; //0 for not moved, 1 for fully moved. 
    [SerializeField] float period = 2f;
    Vector3 StartingPos;
    public override void OnNetworkSpawn()
    {
        StartingPos = playerBody.position;
        
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period; // time.time is framerate Independent 

        const float tau = Mathf.PI * 2;
        float rawSinwave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinwave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        playerBody.position = StartingPos + offset;
    }
}
