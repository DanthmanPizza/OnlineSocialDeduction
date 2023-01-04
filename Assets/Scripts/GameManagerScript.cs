using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        
    }

    int FindNumPlayers() {
        return GameObject.FindGameObjectsWithTag("Player").Length;
    }

    void TurnOrder() {
        int[] turnOrder = new int[FindNumPlayers()];
        for (int i = 0; i < turnOrder.Length; i++) {
            turnOrder[i] = i;
        }
    }

    int[] Shuffle(int[] arr) {
        int[] arrr = new int[arr.Length];
        for (int i = 0; i < arr.Length; i++) {
            int rand = Random.Range(0, arr.Length);
            
        }
        return arrr;
    }
}
