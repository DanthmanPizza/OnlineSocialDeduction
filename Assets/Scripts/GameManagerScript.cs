using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerScript : NetworkBehaviour {

    public int turn;
    public string[] cards;
    public string[] orderOfOperations = {"Doppelganger", "Werewolf", "Minion", "Mason", "Seer", "Robber", "Troublemaker", "Drunk", "Insomniac"};

    void Update() {
        if (Input.GetKeyDown("g")) {
            Shuffle();
            StartGameClientRpc(ArrToString(cards));
        }
    }

    [ClientRpc]
    public void StartGameClientRpc(string caards) {
        CardTime(caards);
        turn = 0;
        TurnTimeClientRpc();
    }

    [ClientRpc]
    public void TurnTimeClientRpc() {
        GameObject[] players = FindPlayers();
        if (turn < players.Length) {
            turn++;
            foreach (GameObject player in players) {
                player.SendMessage("TurnTime", orderOfOperations[turn - 1]);
            }
        }
    }

    GameObject[] FindPlayers() {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    void CardTime(string kards) {
        string[] carbd = kards.Split(" ");
        GameObject[] players = FindPlayers();
        for (int i = 0; i < players.Length; i++) {
            players[i].SendMessage("RecieveCard", carbd[i]);
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

    string ArrToString(string[] str) {
        string store = "";
        foreach (string i in str) {
            store += i + " ";
        }
        return store;
    }
}