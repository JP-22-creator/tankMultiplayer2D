using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{

    private NetworkManager networkManager;

    public NetworkServer(NetworkManager nM)
    {
        networkManager = nM;
        networkManager.ConnectionApprovalCallback += ApprovalCheck; // triggered on connection
                                                                    // can retrive some connection info

    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        UserData userData = Desirealise(request);
        // now we can check if we want to approve
        response.Approved = true;
        // bcs of ConnectionApproval we have to spawn the player here
        response.CreatePlayerObject = true;
        //maybe we can change the spawn position
        Debug.Log("------------------------" + userData.userName);
    }

    private UserData Desirealise(NetworkManager.ConnectionApprovalRequest input)
    {
        string payload = System.Text.Encoding.UTF8.GetString(input.Payload); // Byte -> String 
        return JsonUtility.FromJson<UserData>(payload); // String -> JSON -> UserData

    }


}