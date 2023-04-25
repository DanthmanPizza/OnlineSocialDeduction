using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManagerScript : NetworkBehaviour {

    public int turn;
    public int playersDoneCounter;
    public int[] votes;
    public string[] cards;
    public string[] orderOfOperations = {"Doppelganger", "Werewolf", "Minion", "Mason", "Seer", "Robber", "Troublemaker", "Drunk", "Insomniac", "Voting"};
    public string winner;

    void StartGame() {
        Shuffle();
        StartGameClientRpc(ArrToString(cards));
    }

    [ClientRpc]
    public void StartGameClientRpc(string caards) {
        votes = new int[FindPlayers().Length];
        CardTime(caards);
        turn = 0;
        TurnTimeClientRpc();
    }

    [ClientRpc]
    public void TurnTimeClientRpc() {
        GameObject[] players = FindPlayers();
        if (turn < orderOfOperations.Length) {
            turn++;
            foreach (GameObject player in players) {
                player.SendMessage("MyTurn", orderOfOperations[turn - 1]);
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
        GameObject[] middle = GameObject.FindGameObjectsWithTag("Middle");
        for (int j = 0; j < middle.Length; j++) {
            middle[j].SendMessage("RecieveCard", carbd[j + players.Length]);
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

    int FindIndexOfLargestInArray(int[] arr) {
        int currentLargest = 0;
        int currentLargestLocation = 0;
        bool tie = false;
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == currentLargest) tie = true;
            if (arr[i] > currentLargest) {
                currentLargest = arr[i];
                currentLargestLocation = i;
                tie = false;
            }
        }
        if (!tie) return currentLargestLocation;
        return -1;
    }

    void WhoWon() {
        string deadCards = "";
        bool werewolfPresent = false;
        foreach (GameObject player in FindPlayers()) {
            if (!player.GetComponent<PlayerScript>().alive) deadCards += player.GetComponent<PlayerScript>().card;
            if (player.GetComponent<PlayerScript>().SearchForCard("Werewolf") > 0) werewolfPresent = true;
        }
        bool someoneDied = false;
        if (deadCards != "") someoneDied = true;
        bool tannerDied = false;
        if (deadCards.Contains("Tanner")) tannerDied = true;
        bool werewolfDied = false;
        if (deadCards.Contains("Werewolf")) werewolfDied = true;
        if (tannerDied && werewolfDied) winner = "Tanner And Villagers Win!";
        else if ((!werewolfDied && werewolfPresent && !tannerDied)) winner = "Werewolves Win!";
        else if ((!someoneDied && !werewolfPresent) || werewolfDied) winner = "Villagers Win!";
        else if (tannerDied) winner = "Tanner Wins!";
        else winner = "Nobody Won!";
    }

    [ServerRpc]
    public void TurnOverServerRpc() {
        TurnOverClientRpc();
    }

    [ClientRpc]
    public void TurnOverClientRpc() {
        playersDoneCounter++;
        if (playersDoneCounter >= FindPlayers().Length) {
            playersDoneCounter = 0;
            TurnTimeClientRpc();
        }
    }

    [ServerRpc]
    void VoteServerRpc(int choosedPlayer) {
        VoteClientRpc(choosedPlayer);
    }

    [ClientRpc]
    void VoteClientRpc(int chosenPlayer) {
        votes[chosenPlayer]++;
        if (playersDoneCounter >= FindPlayers().Length - 1) {
            if (FindIndexOfLargestInArray(votes) > -1) {
                GameObject killedPlayer = FindPlayers()[FindIndexOfLargestInArray(votes)];
                killedPlayer.SendMessage("Murdered");
                if (killedPlayer.GetComponent<PlayerScript>().card.Contains("Hunter")) {
                    FindPlayers()[killedPlayer.GetComponent<PlayerScript>().voteStorage].SendMessage("Murdered");
                }
            }
            GameObject.FindGameObjectWithTag("Image").SendMessage("ShowCard");
            WhoWon();
        }
    }
}