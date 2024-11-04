using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] Transform turretTransform;
    [SerializeField] InputReader input;

    private Vector2 Aim;

    private void LateUpdate()
    {
        if (!IsOwner) return;

        Vector2 aimScreenPosition = input.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
        );




    }
}
