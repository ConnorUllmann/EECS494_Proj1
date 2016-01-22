using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

    public new Sprite[] animation;
    private int frame = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<SpriteRenderer>().sprite = animation[(int)((Time.time * 8) % 2)];
    }
}
