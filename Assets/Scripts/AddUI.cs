using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class AddUI : MonoBehaviour {
    
    public string canvas;
	// Use this for initialization
	void Start () {
        transform.GetChild(0).SetParent(GameObject.Find(canvas).transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
}
