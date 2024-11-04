using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class connectButtons : NetworkBehaviour
{
    public Canvas canvas;



    public void Host()
    {
        NetworkManager.Singleton.StartHost();

    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();

    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();

    }

}
