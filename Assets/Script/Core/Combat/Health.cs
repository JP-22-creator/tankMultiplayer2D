using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(); // can only be modified on the Server
    // can only contain Primitives -> use Interface to implement custom types

    [field: SerializeField] public int MaxHealth { get; private set; } = 100; // Property that is editable in editor

    private bool isDead = false;

    public event Action<Health> OnDieEvent;

    public override void OnNetworkSpawn() // for every player connecting, this code will try to run on each connected pc
                                          // when the server runs it, it will set the value, all others return
    {
        if (!IsServer) return;

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    public void RestoreHealth(int heal)
    {
        ModifyHealth(heal);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) return;

        int updatedHealth = value + CurrentHealth.Value;
        CurrentHealth.Value = Mathf.Clamp(updatedHealth, 0, MaxHealth);

        if (CurrentHealth.Value < 0)
        {
            isDead = true;
            OnDieEvent?.Invoke(this);
        }
    }

}
