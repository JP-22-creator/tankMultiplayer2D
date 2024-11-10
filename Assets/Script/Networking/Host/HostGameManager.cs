using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostGameManager
{
    private const int maxConnections = 4;
    private const String gameSceneName = "Game";
    private Allocation allocation;
    private String joinCode;

    public async Task StartHostAsync()
    {

        allocation = await GetAllocation();

        joinCode = await GetJoinCode();

        // process of changing transport too relay if needed!
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>(); // get acces
        RelayServerData relayServerData = new RelayServerData(allocation, "udp"); // package the select
        transport.SetRelayServerData(relayServerData); // set too use relay

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        // Disables clients from chaning scene 
        // Inits the NetworkObjects correctl

    }

    private async Task<Allocation> GetAllocation()
    {
        Allocation alloc;
        try
        {
            alloc = await Relay.Instance.CreateAllocationAsync(maxConnections);// returns the alloc we got
        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
            return null;
        }

        return alloc;
    }

    private async Task<String> GetJoinCode()
    {
        string code;
        try
        {
            code = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId); // returns joinCode for this alloc 
            Debug.Log(code);
        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
            return "";
        }

        return code;
    }


}