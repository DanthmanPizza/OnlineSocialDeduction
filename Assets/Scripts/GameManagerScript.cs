using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    GameObject[] players;

    void Update() {
        if (Input.GetKeyDown("g")) {
            StartGame();
        }
    }

    public void StartGame() {
        FindNumPlayers();
        YourTurn(players[0]);
    }

    int FindNumPlayers() {
        players = GameObject.FindGameObjectsWithTag("Player");
        return players.Length;
    }

    void YourTurn(GameObject whoseTurn) {
        whoseTurn.SendMessage("MyTurn");
    }
}
