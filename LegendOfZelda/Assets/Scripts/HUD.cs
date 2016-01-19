using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    public Text rupee_text;
    public Text health_text;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        rupee_text.text = "Rupees: " + PlayerControl.S.rupee_count.ToString();
        health_text.text = "Health:   " + PlayerControl.S.health.ToString();
	}
}
