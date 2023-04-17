using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardImageHandler : MonoBehaviour {

    void Awake() {
        HideCard();
    }

    void ShowCard() {
        this.GetComponent<Image>().enabled = true;
    }

    void HideCard() {
        this.GetComponent<Image>().enabled = false;
    }

    void ChangeImage(string newImage) {
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>(newImage);
    }

    public void CardClicked() {
		foreach (GameObject pla in GameObject.FindGameObjectsWithTag("Player")) {
			pla.SendMessage("ClickedMyCard");
		}
	}
}
