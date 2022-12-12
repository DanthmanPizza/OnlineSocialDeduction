using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptAlpha : MonoBehaviour {
	public GameObject card;

    // Start is called before the first frame update
    void Awake() {
	    getInPosition(1, 3);
    }

	private void getInPosition(int playerNum, int numPlayers) {
		transform.Rotate(0, playerNum * 360 / numPlayers, 0);
		transform.Translate(0, 0, 7);
		transform.Rotate(0, 180, 0);
	}

    // Update is called once per frame
    void Update() {
	    
    }
}
