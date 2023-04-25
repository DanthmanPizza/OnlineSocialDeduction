using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class AllInButtonScript : MonoBehaviour {

    void Awake() {
        TurnOff();
    }

    void TurnOn() {
        GetComponent<Button>().enabled = true;
        GetComponent<Image>().enabled = true;
        GetComponentInChildren<TextMeshProUGUI>().enabled = true;
    }

    void TurnOff() {
        GetComponent<Button>().enabled = false;
        GetComponent<Image>().enabled = false;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;
    }

    public void Activate() {
        TurnOn();
        this.gameObject.SetActive(true);
    }

    public void Deactivate() {
        TurnOff();
        this.gameObject.SetActive(false);
    }

    public void Pressed() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject manager = GameObject.FindGameObjectWithTag("Manager");
        foreach (GameObject pla in players) {
                pla.SendMessage("StartGame");
        }
        manager.SendMessage("StartGame");
        Deactivate();
    }
}
