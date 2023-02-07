using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerScript : NetworkBehaviour {

    int turn;
    public string[] cards;

    void Update() {
        if (Input.GetKeyDown("g")) {
            StartGameClientRpc();
        }
    }

    [ClientRpc]
    public void StartGameClientRpc() {
        CardTime();
        turn = 0;
        TurnTime();
    }

    public void TurnTime() {
        GameObject[] players = FindPlayers();
        if (turn < players.Length) {
            turn++;
            players[turn - 1].BroadcastMessage("MyTurn");
        }
    }

    GameObject[] FindPlayers() {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    void CardTime() {
        Shuffle();
        GameObject[] players = FindPlayers();
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