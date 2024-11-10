using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{

    private const string MenuSceneName = "Menu";
    private JoinAllocation joinAllocation;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync(); // very important

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public async Task StartClientAsync(string joinCode)
    {
        joinAllocation = await GetAlloction(joinCode);
        if (joinAllocation == null) return;

        // setup too use Relay
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAllocation, "udp");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
        // this should already change the Scene 
    }


    private async Task<JoinAllocation> GetAlloction(string code)
    {
        JoinAllocation alloc;
        try
        {
            alloc = await Relay.Instance.JoinAllocationAsync(code);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        return alloc;
    }



    public void GoToMenu()
    {
        // Authenticate player
        SceneManager.LoadScene(MenuSceneName);
    }
}
