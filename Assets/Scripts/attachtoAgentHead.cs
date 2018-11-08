using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attachtoAgentHead : MonoBehaviour {

    public GameObject Agent;
	// Use this for initialization
	void Start () {
        transform.position = Agent.transform.position+new Vector3(0,1,0);
        //transform.SetParent(Agent.transform);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Agent.transform.position + new Vector3(0, 2.5f, 0);
    }
}
