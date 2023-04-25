using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour {
    
    [SerializeField]private Button hostButton;
    [SerializeField]private Button clientButton;
    
    private void Awake() {
        hostButton.onClick.AddListener(() => {
            DisableButtons();
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() => {
            DisableButtons();
            NetworkManager.Singleton.StartClient();
        });
    }

    private void DisableButtons() {
        clientButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
    }

}