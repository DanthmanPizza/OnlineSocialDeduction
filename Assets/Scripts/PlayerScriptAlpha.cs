using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScriptAlpha : NetworkBehaviour {
	int playerNum;
	int numPlayers;
	public string card;
	public int startGameMovement;
	public bool myTurn = false;
	public string seenCard;

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
	public void MyTurnClientRpc(bool turnOver) {
		if (!turnOver || myTurn) {
			Debug.Log("Turn Recieved");
			myTurn = !myTurn;
			if (turnOver) {
				GameObject.FindGameObjectWithTag("Manager").SendMessage("TurnTimeClientRpc");
			}
		}
	}

	public void RecieveCard(string carb) {
		card = carb;
	}

	public void Robber() {
		if (!card.Contains("robber")) return;
		//Robbing code goes here.
	}

	public void Seer(int selectedPlayer) {
		if (!card.Contains("seer")) return;
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (selectedPlayer == ExtractPlayerNumber(pla)) {
				seenCard = pla.GetComponent<PlayerScriptAlpha>().card;
			}
		}
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
			Debug.Log("Turn Sent");
			Seer(plaNum);
			if (!IsHost && !IsServer) {
				ClientTurnServerRpc();
			}
			else {
				MyTurnClientRpc(true);
			}
		}
	}

	int ExtractPlayerNumber(GameObject ploe) {
		return ploe.GetComponent<PlayerScriptAlpha>().playerNum;
	}

	[ServerRpc]
	void ClientTurnServerRpc() {
		MyTurnClientRpc(true);
	}
}