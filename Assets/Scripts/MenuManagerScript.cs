using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MenuManagerScript : NetworkBehaviour
{
    
    public void StartGame() {
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartHost();
    }

    public void JoinGame() {
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartClient();
    }
}
