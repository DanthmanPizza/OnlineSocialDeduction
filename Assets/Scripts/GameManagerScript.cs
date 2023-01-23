using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public GameObject[] players;
    public int turn;
    
    void Update() {
        if (Input.GetKeyDown("g")) {
            StartGame();
        }
    }

    public void StartGame() {
        FindPlayers();
        turn = 0;
        TurnTime();
    }

    public void TurnTime() {
        if (turn < players.Length) {
            turn++;
            players[turn - 1].SendMessage("MyTurn");
        }
    }

    void FindPlayers() {
        players = GameObject.FindGameObjectsWithTag("Player");
    }
}
