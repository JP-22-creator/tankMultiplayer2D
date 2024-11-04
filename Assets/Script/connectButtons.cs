using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class connectButtons : NetworkBehaviour
{
    public Canvas canvas;

    private void DeactivateCanvas()
    {
        canvas.gameObject.SetActive(false);
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateCanvas();
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateCanvas();
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
        DeactivateCanvas();
    }

}
