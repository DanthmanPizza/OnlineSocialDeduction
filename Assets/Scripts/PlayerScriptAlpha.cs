using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScriptAlpha : NetworkBehaviour {
	int playerNum;
	int numPlayers;
	public string card;
	public int startGameMovement;

    	//i am severerly out of my depth
	// Start is called before the first frame update
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
		CameraOnOff();
	}

	public void GetInPosition() {
		transform.Rotate(0, playerNum * 360 / (numPlayers + 1), 0);
		transform.Translate(0, 0, startGameMovement);
		transform.Rotate(0, 180, 0);
	}

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}

	public void MyTurn() {
		GameObject.FindGameObjectWithTag("Manager").SendMessage("TurnTime");
	}

	public void RecieveCard(string carb) {
		card = carb;
	}

	public void Robber() {
		if (!card.Contains("robber")) return;
		//Robbing code goes here.
	}

	public void Seer() {
		if (!card.Contains("seer")) return;
		//Seeing code goes here.
	}

	void CameraOnOff() {
        if (IsLocalPlayer) return;
        this.GetComponent<Camera>().enabled = false;
    }
}