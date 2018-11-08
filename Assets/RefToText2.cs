using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RefToText2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManagement.RedHouseText2 = GetComponent<Text>().gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
