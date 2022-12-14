﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptAlpha : MonoBehaviour {
	public GameObject card;
	public int playerNum = 1;
	public int numPlayers = 5;

    // Start is called before the first frame update
	void Awake() {
		getInPosition();
    }

	// Update is called once per frame
	void Update() {
	    
	}

	public void getInPosition() {
		transform.Rotate(0, playerNum * 360 / numPlayers, 0);
		transform.Translate(0, 0, 7);
		transform.Rotate(0, 180, 0);
	}
}