using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptAlpha : MonoBehaviour {
	public GameObject card;
	public int playerNum;
	public int numPlayers;

    // Start is called before the first frame update
	void Awake() {
		playerNum = PlayerNumFinder();
    }

	// Update is called once per frame
	void StartGame() {
	    numPlayers = PlayerNumFinder();
		getInPosition();
	}

	public void getInPosition() {
		
		transform.Rotate(0, playerNum * 360 / numPlayers, 0);
		transform.Translate(0, 0, 7);
		transform.Rotate(0, 180, 0);
	}

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}
}