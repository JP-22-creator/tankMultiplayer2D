using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private Health health;

    [SerializeField] private Image healthBar;


    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value); // catch pre subscribe change
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth) // OnValueChanged parses 2 params old and new val
    {
        healthBar.fillAmount = (float)newHealth / health.MaxHealth;
    }

}
