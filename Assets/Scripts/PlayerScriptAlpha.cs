using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScriptAlpha : NetworkBehaviour {
	int playerNum;
	int numPlayers;
	public string originalCard = "";
	public string card;
	public int startGameMovement;
	public bool myTurn = false;
	public string seenCard;
	public string currentCard;


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
		numPlayers = PlayerNumFinder();
		GetInPosition();
		CameraOnOffClientRpc();
	}

	public void GetInPosition() {
		transform.Rotate(0, playerNum * 360 / (numPlayers + 1), 0);
		transform.Translate(0, 0, startGameMovement);
		transform.Rotate(0, 180, 0);
	}

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}

	[ClientRpc]
	public void MyTurnClientRpc(string whatsOccuring) {
		myTurn = true;
		currentCard = whatsOccuring;
		if (!originalCard.Contains(whatsOccuring)) {
			myTurn = false;
			Debug.Log("Telling the Manager" + playerNum);
			TellTheManagerServerRpc("TurnOverClientRpc");
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
		seenCard = ViewCard(selectedPlayer);
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
		if (!IsLocalPlayer) return;
		if (myTurn) {
			Seer(plaNum);
			Robber(plaNum);
			TurnToggleClientRpc();
			Debug.Log("Telling the Manager" + plaNum);
			TellTheManagerServerRpc("TurnOverClientRpc");
		}
	}

	int ExtractPlayerNumber(GameObject ploe) {
		return ploe.GetComponent<PlayerScriptAlpha>().playerNum;
	}

	string ViewCard(int chosenPlayer) {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (chosenPlayer == ExtractPlayerNumber(pla)) {
				return pla.GetComponent<PlayerScriptAlpha>().card;
			}
		}
		return "error";
	}

	[ServerRpc]
	void ChangingCardServerRpc(string newCard, int plaNom) {
		ChangingCardClientRpc(newCard, plaNom);
	}

	[ClientRpc]
	void ChangingCardClientRpc(string newCard, int palNumbre) {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (palNumbre == ExtractPlayerNumber(pla)) {
				pla.GetComponent<PlayerScriptAlpha>().card = newCard;
			}
		}
	}

	[ClientRpc]
	void TurnToggleClientRpc() {
		myTurn = !myTurn;
	}

	[ServerRpc(RequireOwnership = false)]
	public void TellTheManagerServerRpc(string whatToSay) {
		GameObject.FindGameObjectWithTag("Manager").SendMessage(whatToSay);
	}

	/* preserved for posterity
	[ClientRpc]
	public void MyTurnClientRpc(bool turnOver) {
		if (!turnOver || myTurn) {
			myTurn = !myTurn;
			if (turnOver) {
				GameObject.FindGameObjectWithTag("Manager").SendMessage("TurnTimeClientRpc");
			}
		}
	}

	[ServerRpc]
	void ClientTurnServerRpc(bool turnYN) {
		MyTurnClientRpc(true);
	}
	*/
}