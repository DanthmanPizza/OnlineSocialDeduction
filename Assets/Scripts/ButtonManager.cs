using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : NetworkBehaviour {

	public string ip;
	public int port;
	[SerializeField]private Button joinButton;
	
	void Awake() {
		
	}
}
