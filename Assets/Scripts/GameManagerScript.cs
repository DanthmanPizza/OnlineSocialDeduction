using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerScript : NetworkBehaviour {

    GameObject[] players;
    int turn;
    public string[] cards;
    
    void Update() {
        if (Input.GetKeyDown("g")) {
            StartGame();
        }
    }

    public void StartGame() {
        FindPlayers();
        CardTime();
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

    void CardTime() {
        Shuffle();
        for (int i = 0; i < players.Length; i++) {
            players[i].SendMessage("RecieveCard", cards[i]);
        }
    }

    void Shuffle() {
        string temp;
        for (int i = 0; i < cards.Length; i++) {
            int rand = Random.Range(i, cards.Length);
            temp = cards[rand];
            cards[rand] = cards[i];
            cards[i] = temp;
        }
    }
}