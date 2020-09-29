using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEmerald : MonoBehaviour
{
    // Start is called before the first frame update


    float yOriginal;
    public float forceFlote = 1;

    void Start()
    {
        this.yOriginal = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yOriginal + ((float)Math.Sin(Time.time) * forceFlote), transform.position.z);
    }
}