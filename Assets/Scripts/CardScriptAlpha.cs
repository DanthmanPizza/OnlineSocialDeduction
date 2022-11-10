using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardScriptAlpha : MonoBehaviour {
    private string[] properties;

    public void cardMaker(string props) {
        properties = props.Split();
    }
}