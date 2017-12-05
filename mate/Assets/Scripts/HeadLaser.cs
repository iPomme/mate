using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * from https://www.youtube.com/watch?v=ZT8eutqzW5A
 * 
 **/
public class HeadLaser : MonoBehaviour {


    LineRenderer lineRend;

    public Transform startPos;
    public Transform endPos;

    private float textureOffset = 0.0f;

	// Use this for initialization
	void Start () {
        lineRend = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        // Update the spot position
        lineRend.SetPosition(0, startPos.position);
        lineRend.SetPosition(1, endPos.position);

        // pan texture
        textureOffset -= Time.deltaTime * 2.0f;

        if(textureOffset < -10.0f){
            textureOffset += 10f;
        }

        lineRend.sharedMaterials[1].SetTextureOffset("_MainTex", new Vector2(textureOffset,0f));
	}
}
