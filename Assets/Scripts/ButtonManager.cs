using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
	public string ip;
	public int port;
	[SerializeField]private Button joinButton;
	
	void Awake() {
		
	}
}
