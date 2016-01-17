using UnityEngine;
using System.Collections;

public class DisappearOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().enabled = false;	
	}

}
