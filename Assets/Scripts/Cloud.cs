using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour {
    
    [Range(0.15f, 10f)]
    public float cloudSpeed;

    private const float Bound = 200f;


    private void Start() {
        // allow each cloud to have it's own speed
        cloudSpeed += Random.Range(-0.15f, 0f);
    }

    void Update() {
        var pos = transform.position;
        if (pos.z > Bound) {
            pos += new Vector3(0, 0, -Bound * 2f);
        }
        pos += Vector3.forward * cloudSpeed;
        transform.position = pos;
    }
}