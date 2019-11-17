using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDome : MonoBehaviour {
    
    public float offsetSpeed = 0.001f;

    private Renderer rend;
    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<Renderer> ();
    }

    // Update is called once per frame
    void Update() {
        var material = rend.material;
        var offset = material.mainTextureOffset;
        offset += offsetSpeed * Time.deltaTime * Vector2.right;
        material.mainTextureOffset = offset;
    }
}
