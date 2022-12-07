using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptAlpha : MonoBehaviour {
    public GameObject card;

    // Start is called before the first frame update
    void Awake() {
        
    }

    // Update is called once per frame
    void Update() {
	    Move((float)0.01);
    }
    
	public void Move(float moveLength) {
		transform.Translate(new Vector3(moveLength, moveLength, moveLength));
	}
}
