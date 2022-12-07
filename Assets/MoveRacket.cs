using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRacket : MonoBehaviour
{
    //a float is a decimal
    public float speed = 30;
    public string axis = "Vertical";

    // FixedUpdate() is called 30fps
    void FixedUpdate()
    {
        //GetAxisRaw returns either -1, 0, or 1 depending on button press
        float v = Input.GetAxisRaw(axis);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, v) * speed;
    }
}

// This is a comment
// Hello!

/* 
    Anything in here is a comment
*/