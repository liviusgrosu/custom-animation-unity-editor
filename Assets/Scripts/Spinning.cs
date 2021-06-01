﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour {

    [Header("Spinning")]
    public float turnSpeed;
    public bool x, y, z;
    private void Start()
    {
    }

    private void Update() {
        //Spin the object depending on what axis the user requested in edit mode
        if (x) transform.Rotate(turnSpeed, 0, 0);
        if (y) transform.Rotate(0, turnSpeed, 0);
        if (z) transform.Rotate(0, 0, turnSpeed);
    }
}
