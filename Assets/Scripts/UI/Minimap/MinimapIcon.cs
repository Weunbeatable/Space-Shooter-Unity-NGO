using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MinimapIcon : NetworkBehaviour
{
    // Start is called before the first frame update
    public Rigidbody player;
    [SerializeField] private InputReader inputReader;
    Vector3 previousMovementInput;
    [SerializeField] private float turningRate = 30f;
    float yRotation;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += HandleMove;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMove;
    }
    private void HandleMove(Vector2 movementInput)
    {
        //  previousMovementInput = movementInput;
        previousMovementInput = new Vector3(movementInput.x, movementInput.y, 0f).normalized;
    }
    void Update()
    {
        if (!IsOwner) return;
        // input of our device * -turn rate so its not inverted and make framerate independent
        yRotation = previousMovementInput.x * -turningRate * Time.deltaTime;

    }
    private void LateUpdate()
    {
        if (!IsOwner) { return; }
        transform.Rotate(0f, 0f, -yRotation);
    }
}
