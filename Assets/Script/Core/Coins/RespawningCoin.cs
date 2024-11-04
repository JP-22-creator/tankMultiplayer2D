using System;
using UnityEngine;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> OnCollectedEvent;

    private Vector2 previousPosition;

    public override void OnNetworkSpawn()
    {
        previousPosition = (Vector2)transform.position;
    }

    private void Update()
    {
        if (previousPosition != (Vector2)transform.position)
        {
            Show(true);
            previousPosition = (Vector2)transform.position;
        }
    }

    public override int Collect()
    {
        if (!IsServer) // clients run this part 
        {
            Show(false);
            return 0;
        }

        if (isCollected) return 0;

        isCollected = true;

        OnCollectedEvent?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        isCollected = false;
        Show(true);
    }



}