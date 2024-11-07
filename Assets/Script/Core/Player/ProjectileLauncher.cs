using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{

    [SerializeField] private GameObject serverProjectile;
    [SerializeField] private GameObject clientProjectile;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed;

    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;




    [SerializeField] private InputReader input;



    private bool shouldFire;
    private float muzzleFlashTimer;

    private float timer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        input.OnPrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        input.OnPrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update()
    {

        HandleMuzzleFlash();



        if (!IsOwner) return;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }
        if (!shouldFire) return;



        timer = 1 / fireRate;

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up); // client sees local bullet

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up); // server double checks values
        // calls the clientRpc





    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject go = Instantiate(clientProjectile, spawnPosition, Quaternion.identity);
        go.transform.up = direction; // let the projectile face the same direc as turret

        if (go.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) // forward movement of projectile
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject go = Instantiate(serverProjectile, spawnPosition, Quaternion.identity);
        go.transform.up = direction;


        if (go.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPosition, direction);
    }

    [ClientRpc] // spawns the bullet for everyone else
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPosition, Vector3 direction)
    {
        if (IsOwner) return; // if we are the owner we already spawned the dummy projectile before!
        SpawnDummyProjectile(spawnPosition, direction);

    }


    private void HandleMuzzleFlash()
    {
        if (muzzleFlashTimer > 0) muzzleFlashTimer -= Time.deltaTime;
        if (muzzleFlashTimer <= 0f) muzzleFlash.SetActive(false);
    }


    private void HandlePrimaryFire(bool fire)
    {
        shouldFire = fire;
    }



}
