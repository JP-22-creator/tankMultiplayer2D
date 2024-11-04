using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private Transform treadsTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float turnSpeed = 90;

    private Vector2 previousMovementInput;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // only run on owner
        input.OnMoveEvent += HandleMove;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        input.OnMoveEvent -= HandleMove;
    }

    private void Update() // Playermovement is Client auth !
    {
        float zRotation = previousMovementInput.x * -turnSpeed * Time.deltaTime; // x is a and d || a = -1 d = 1 
        treadsTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate() // Physics engine update
    {
        if (!IsOwner) return;

        rb.velocity = moveSpeed * previousMovementInput.y * (Vector2)treadsTransform.up; // no need to * Time.delta time bcs fixedUpdate
    }



    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
}
