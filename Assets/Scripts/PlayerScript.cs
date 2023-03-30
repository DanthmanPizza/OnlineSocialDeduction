using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour {
	
	public int playerNum;
	public int numPlayers;
	public string originalCard = "";
	public string card;
	public int startGameMovement;
	public bool myTurn = false;
	public string seenCard;
	public string seenCardTwo;
	public string currentCard;
	public bool alive = true;
	bool sawMiddle = false;
	bool youNeedAnotherTry = false;
	int selectedPlayerStorage = -1;
	string selectedPlayerCardStorage = "";


		//i am severerly out of my depth
	void Awake() {
		playerNum = PlayerNumFinder();
	}

	void Update() {
		if (Input.GetKeyDown("g")) {
			StartGame();
		}
	}

	void StartGame() {
		StartGameClientRpc();
		GetInPosition();
		CameraOnOffClientRpc();
	}

	public void GetInPosition() {
		transform.Rotate(0, playerNum * 360 / (numPlayers + 1), 0);
		transform.Translate(0, 0, startGameMovement);
		transform.Rotate(0, 180, 0);
	}

	[ClientRpc]
	void StartGameClientRpc() {
		alive = true;
		numPlayers = PlayerNumFinder();
	}

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}

	public void MyTurn(string whatsOccuring) {
		myTurn = true;
		currentCard = whatsOccuring;
		Drunk();
		if (!originalCard.Contains(whatsOccuring) && whatsOccuring != "Voting") {
			if (IsOwner) TurnThingsServerRpc();
		}
	}

	public void RecieveCard(string carb) {
		card = carb;
		if (originalCard == "") originalCard = carb;
	}

	public void Robber(int selectedPlayer) {
		if (!originalCard.Contains("Robber") || currentCard != "Robber") return;
		string myNewCard = ViewCard(selectedPlayer);
		ChangingCardServerRpc(card, selectedPlayer);
		ChangingCardServerRpc(myNewCard, playerNum);
	}

	public void Seer(int selectedPlayer) {
		if (!originalCard.Contains("Seer") || currentCard != "Seer") return;
		if (sawMiddle) {
			sawMiddle = false;
			if (selectedPlayer <= PlayerNumFinder()) {
				youNeedAnotherTry = true;
				return;
			}
			seenCardTwo = ViewCard(selectedPlayer);
			return;
		}
		if (selectedPlayer > PlayerNumFinder()) {
			sawMiddle = true;
			youNeedAnotherTry = true;
		}
		seenCard = ViewCard(selectedPlayer);
	}

	public void Drunk() {
		if (!originalCard.Contains("Drunk") || currentCard != "Drunk" || !IsLocalPlayer) return;
		int rand = Random.Range(1, 4);
		string tempCard = card;
		ChangingCardServerRpc(ViewCard(numPlayers + rand), playerNum);
		ChangingMiddleCardServerRpc(tempCard, rand);
		TurnThingsServerRpc();
	}

	void Troublemaker(int selectedPlayer) {
		if (!originalCard.Contains("Troublemaker") || currentCard != "Troublemaker") return;
		if (selectedPlayerStorage > -1) {
			ChangingCardServerRpc(ViewCard(selectedPlayer), selectedPlayerStorage);
			ChangingCardServerRpc(selectedPlayerCardStorage, selectedPlayer);
			selectedPlayerStorage = -1;
			selectedPlayerCardStorage = "";
		}
		else {
			selectedPlayerStorage = selectedPlayer;
			selectedPlayerCardStorage = ViewCard(selectedPlayer);
			youNeedAnotherTry = true;
		}
	}

	void Mason(int selectedPlayer) {
		if (!originalCard.Contains("Mason") || currentCard != "Mason") return;
		if (!ViewCard(selectedPlayer).Contains("Mason") && SearchForCard("Mason") > 1) {
			youNeedAnotherTry = true;
		}
	}

	void Werewolf(int selectedPlayer) {
		if (!originalCard.Contains("Werewolf") || currentCard != "Werewolf") return;
		if (SearchForCard("Werewolf") == 1) {
			if (selectedPlayer <= numPlayers) {
				youNeedAnotherTry = true;
				return;
			}
			else {
				seenCard = ViewCard(selectedPlayer);
				return;
			}
		}
		if (!ViewCard(selectedPlayer).Contains("Werewolf") && SearchForCard("Werewolf") > 1) {
			youNeedAnotherTry = true;
		}
	}

	public void Voting(int selectedPlayer) {
		if (currentCard != "Voting") return;
		RegisterVoteServerRpc(selectedPlayer);
	}

	[ClientRpc]
	void CameraOnOffClientRpc() {
		if (IsLocalPlayer) return;
		this.GetComponent<Camera>().enabled = false;
	}

	void OnMouseDown() {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			pla.SendMessage("Pressed", playerNum);
		}
	}

	void Pressed(int plaNum) {
		if (!IsLocalPlayer || !myTurn) return;
		Seer(plaNum);
		Werewolf(plaNum);
		if (plaNum <= PlayerNumFinder()) {
			Mason(plaNum);
			Robber(plaNum);
			Troublemaker(plaNum);
			Voting(plaNum);
		}
		if (!CheckIfMiddleCard(currentCard) && plaNum > PlayerNumFinder()) youNeedAnotherTry = true;
		if (youNeedAnotherTry) {
			youNeedAnotherTry = false;
			return;
		}
		TurnThingsServerRpc();
	}

	int ExtractPlayerNumber(GameObject ploe) {
		return ploe.GetComponent<PlayerScript>().playerNum;
	}

	string ViewCard(int chosenPlayer) {
		if (chosenPlayer > PlayerNumFinder()) {
			foreach (GameObject mid in GameObject.FindGameObjectsWithTag("Middle")) {
				if (chosenPlayer - PlayerNumFinder() == mid.GetComponent<MiddleScript>().myNumber) {
					return mid.GetComponent<MiddleScript>().card;
				}
			}
		}
		else {
			foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
				if (chosenPlayer == ExtractPlayerNumber(pla)) {
					return pla.GetComponent<PlayerScript>().card;
				}
			}
		}
		return "error";
	}

	public int SearchForCard(string searchCard) {
		int i = 0;
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (pla.GetComponent<PlayerScript>().card.Contains(searchCard)) i++;
		}
		return i;
	}

	[ServerRpc]
	void ChangingCardServerRpc(string newCard, int plaNom) {
		ChangingCardClientRpc(newCard, plaNom);
	}

	[ClientRpc]
	void ChangingCardClientRpc(string newCard, int palNumbre) {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (palNumbre == ExtractPlayerNumber(pla)) {
				pla.GetComponent<PlayerScript>().card = newCard;
			}
		}
	}

	[ServerRpc]
	void ChangingMiddleCardServerRpc(string newCard, int midNum) {
		ChangingMiddleCardClientRpc(newCard, midNum);
	}

	[ClientRpc]
	void ChangingMiddleCardClientRpc(string newCard, int midNum) {
		foreach (GameObject mid in GameObject.FindGameObjectsWithTag("Middle")) {
			if (midNum == mid.GetComponent<MiddleScript>().myNumber) {
				mid.GetComponent<MiddleScript>().card = newCard;
			}
		}
	}

	void TurnToggle() {
		myTurn = !myTurn;
	}

	public void Murdered() {
		alive = false;
	}

	bool CheckIfMiddleCard(string card) {
		if (card.Contains("Seer") || card.Contains("Werewolf")) return true;
		return false;
	}

	[ServerRpc]
	public void TurnThingsServerRpc() {
		TurnThingsClientRpc();
		GameObject.FindGameObjectWithTag("Manager").SendMessage("TurnOverServerRpc");
	}

	[ClientRpc]
	public void TurnThingsClientRpc() {
		TurnToggle();
	}

	[ServerRpc]
	void RegisterVoteServerRpc(int chosenPlayer) {
		GameObject.FindGameObjectWithTag("Manager").SendMessage("VoteServerRpc", chosenPlayer);
	}
}