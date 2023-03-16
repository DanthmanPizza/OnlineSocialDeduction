using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MiddleScript : NetworkBehaviour {

    public string card;
    public string originalCard = "";
    public int myNumber;

	public int PlayerNumFinder() {
		return GameObject.FindGameObjectsWithTag("Player").Length - 1;
	}

	public void RecieveCard(string carb) {
		card = carb;
		if (originalCard == "") originalCard = carb;
	}

	void OnMouseDown() {
        //I will need different code here to accomodate the functions of cards
		/*foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			pla.SendMessage("Pressed", myNumber + PlayerNumFinder());
		}*/
	}
}
