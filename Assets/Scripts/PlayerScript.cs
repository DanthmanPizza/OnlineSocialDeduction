using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
	public int voteStorage;
	public int[] werewolves;

		//i am severerly out of my depth
	void Awake() {
		playerNum = PlayerNumFinder();
	}

	//I despise that I have to use Awake and Start. I am so mad. absolutely livid.
	void Start() {
		if (IsHost) GameObject.FindGameObjectWithTag("AllInButton").SendMessage("Activate");
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
		Minion();
		Drunk();
		Insomniac(false);
		if (IsOwner && originalCard.Contains("Doppelganger") && DoppelInstant(originalCard) && originalCard.Contains(whatsOccuring)) {
			TurnThingsServerRpc();
		}
		if (!originalCard.Contains(currentCard) && !currentCard.Equals("Voting") && IsOwner) {
			TurnThingsServerRpc();
		}
	}

	public void RecieveCard(string carb) {
		card = carb;
		if (IsOwner) ChangeCardImageServerRpc();
		if (originalCard.Equals("")) originalCard = carb;
	}

	public void Robber(int selectedPlayer) {
		if (!originalCard.Contains("Robber") || !(currentCard.Equals("Robber") || currentCard.Equals("Doppelganger"))) return;
		if (originalCard.Contains("Doppelganger") && currentCard.Equals("Robber")) return;
		string myNewCard = ViewCard(selectedPlayer);
		ChangingCardServerRpc(card, selectedPlayer);
		ChangingCardServerRpc(myNewCard, playerNum);
	}

	public void Seer(int selectedPlayer) {
		if (!originalCard.Contains("Seer") || !(currentCard.Equals("Seer") || currentCard.Equals("Doppelganger"))) return;
		if (originalCard.Contains("Doppelganger") && currentCard.Equals("Seer")) return;
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
		if (!originalCard.Contains("Drunk") || !(currentCard.Equals("Drunk") || currentCard.Equals("Doppelganger")) || !IsLocalPlayer) return;
		if (originalCard.Contains("Doppelganger") && currentCard.Equals("Drunk")) return;
		if (originalCard.Contains("Drunk") && !originalCard.Contains("Doppelganger") && currentCard.Equals("Doppelganger")) return;
		int rand = Random.Range(1, 4);
		string tempCard = card;
		ChangingCardServerRpc(ViewCard(numPlayers + rand), playerNum);
		ChangingMiddleCardServerRpc(tempCard, rand);
		TurnThingsServerRpc();
	}

	void Troublemaker(int selectedPlayer) {
		if (!originalCard.Contains("Troublemaker") || !(currentCard.Equals("Troublemaker") || currentCard.Equals("Doppelganger"))) return;
		if (originalCard.Contains("Doppelganger") && currentCard.Equals("Troublemaker")) return;
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

	void Doppelganger(int selectedPlayer) {
		if (!originalCard.Contains("Doppelganger") || !currentCard.Equals("Doppelganger")) return;
		if (DoppelInstant(card)) {
			return;
		}
		string doppelCard = ViewCard(selectedPlayer);
		ChangingOriginalCardServerRpc(card + doppelCard, playerNum);
		ChangingCardServerRpc(card + doppelCard, playerNum);
		if (doppelCard.Contains("Drunk")) Drunk();
		if (DoppelInstant(doppelCard)) {
			youNeedAnotherTry = true;
		}
	}

	void Mason(int selectedPlayer) {
		if (!originalCard.Contains("Mason") || !currentCard.Equals("Mason")) return;
		if (!ViewCard(selectedPlayer).Contains("Mason") && SearchForCard("Mason") > 1) {
			youNeedAnotherTry = true;
		}
	}

	void Minion() {
		if (!originalCard.Contains("Minion") || !currentCard.Equals("Minion") || !IsLocalPlayer) return;
		int i = 0;
		int[] werewolfs = new int[SearchForCard("Werewolf")];
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (ViewCard(ExtractPlayerNumber(pla)).Contains("Werewolf")) {
				werewolfs[i] = ExtractPlayerNumber(pla);
				i++;
			}
		}
		werewolves = werewolfs;
		TurnThingsServerRpc();
	}

	void Werewolf(int selectedPlayer) {
		if (!originalCard.Contains("Werewolf") || !currentCard.Equals("Werewolf")) return;
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

	void Insomniac(bool finished) {
		if (!originalCard.Contains("Insomniac") || !currentCard.Equals("Insomniac") || !IsOwner) return;
		if (finished) {
			GameObject.FindGameObjectWithTag("Image").SendMessage("HideCard");
			TurnThingsServerRpc();
		}
		else {
			GameObject.FindGameObjectWithTag("Image").SendMessage("ShowCard");
		}
	}

	public void Voting(int selectedPlayer) {
		if (!currentCard.Equals("Voting")) return;
		RegisterVoteServerRpc(selectedPlayer);
	}

	[ClientRpc]
	void CameraOnOffClientRpc() {
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
		if (IsLocalPlayer) return;
		this.GetComponent<Camera>().enabled = false;
	}

	void OnMouseDown() {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			pla.SendMessage("Pressed", playerNum);
		}
	}

	void ClickedMyCard() {
		Insomniac(true);
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
		Doppelganger(plaNum);
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
				if (IsOwner) ChangeCardImageServerRpc();
			}
		}
	}

	[ServerRpc]
	void ChangingOriginalCardServerRpc(string newCard, int playNumre) {
		ChangingOriginalCardClientRpc(newCard, playNumre);
	}

	[ClientRpc]
	void ChangingOriginalCardClientRpc(string newCard, int playNumre) {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			if (playNumre == ExtractPlayerNumber(pla)) {
				pla.GetComponent<PlayerScript>().originalCard = newCard;
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

	[ServerRpc]
	void ChangeCardImageServerRpc() {
		ChangeCardImageClientRpc();
	}

	[ClientRpc]
	void ChangeCardImageClientRpc() {
		if (IsLocalPlayer)  {
			GameObject.FindGameObjectWithTag("Image").SendMessage("ChangeImage", card);
		}
	}

	void TurnToggle() {
		myTurn = !myTurn;
	}

	public void Murdered() {
		alive = false;
	}

	bool DoppelInstant(string carv) {
		if (carv.Contains("Seer") || 
		carv.Contains("Robber") ||
		carv.Contains("Troublemaker") ||
		carv.Contains("Drunk")) return true;
		return false;
	}

	bool CheckIfMiddleCard(string card) {
		if (originalCard.Contains("Seer") || originalCard.Contains("Werewolf")) return true;
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
		voteStorage = chosenPlayer;
		GameObject.FindGameObjectWithTag("Manager").SendMessage("VoteServerRpc", chosenPlayer);
	}
}