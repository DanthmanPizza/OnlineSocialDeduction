using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    GameObject[] players;
    int turn;
    

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
        YourTurn(players[turn]);
        turn++;
    }

    void FindPlayers() {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void YourTurn(GameObject whoseTurn) {
        whoseTurn.SendMessage("MyTurn");
    }
}
