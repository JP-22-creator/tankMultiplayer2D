using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Collections;

public class HostGameManager
{
    private const int maxConnections = 4;
    private const String gameSceneName = "Game";
    private Allocation allocation;
    private String joinCode;
    private String lobbyID;

    public async Task StartHostAsync()
    {

        allocation = await GetAllocation();
        if (allocation == null) return;

        joinCode = await GetJoinCode();


        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<String, DataObject>() // parse information too Lobbie object on network
            {
                {
                    "JoinCode", // key
                    new DataObject( // value
                    visibility: DataObject.VisibilityOptions.Member, // TODO look into Member privat public
                    value: joinCode
                    )
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", maxConnections, lobbyOptions);
            lobbyID = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HearthBeatLobby(15f));
        }
        catch (LobbyServiceException LSE)
        {
            Debug.LogError(LSE.Message);
            return;
        }



        // process of changing transport too relay if needed!
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>(); // get acces
        RelayServerData relayServerData = new RelayServerData(allocation, "udp"); // package the select
        transport.SetRelayServerData(relayServerData); // set too use relay

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        // Disables clients from chaning scene 
        // When a client connects to server it will be brought too this scene
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


    private IEnumerator HearthBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID); // has to ping too keep  the lobbie exisiting
            yield return delay;
        }
    }


}