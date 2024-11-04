using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coin;
    [SerializeField] private int maxCoins = 5;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange; // x bsp 500 / 500
    [SerializeField] private Vector2 ySpawnRange; // y -500 / 500

    [SerializeField] private LayerMask layerMask;

    private float coinRadius;

    private Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinRadius = coin.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }

    }


    private void SpawnCoin()
    {
        RespawningCoin go = Instantiate(coin, GetSpawnPoint(), Quaternion.identity);
        go.SetValue(coinValue);
        go.GetComponent<NetworkObject>().Spawn(); // spawn the coin on the Network (owned by server)

        go.OnCollectedEvent += HandleCoinCollected;

    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y); // get potential spawn position

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0) // try putting a circle at spawnPoint, if it doesnt hit anything we are good to go
            {
                return spawnPoint;
            }
        }
    }


}
