using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptAlpha : MonoBehaviour {
	public GameObject card;
	public int playerNum;
	public int numPlayers;

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
	}

	public void GetInPosition() {
		transform.Rotate(0, playerNum * 360 / (numPlayers + 1), 0);
		transform.Translate(0, 0, 314);
		transform.Rotate(0, 180, 0);
	}

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}

	public void MyTurn() {
		GameObject.FindGameObjectWithTag("Manager").SendMessage("TurnTime");
	}
}