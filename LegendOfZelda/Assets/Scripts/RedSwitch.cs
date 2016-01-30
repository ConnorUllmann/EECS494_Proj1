using UnityEngine;
using System.Collections;

public class RedSwitch : MonoBehaviour {
    public static bool areOpen;
    public GameObject particle;
    private GameObject temp;


	// Use this for initialization
	void Start () {
        areOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(!areOpen) {
            temp = Instantiate(particle, transform.position, new Quaternion()) as GameObject;
        }
	}

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag == "Hero" ||
           coll.gameObject.tag == "Boomerang" ||
           coll.gameObject.tag == "Arrow") {

            areOpen = true;
            GreenSwitch.areOpen = false;

            foreach(Tile t in Tile.redDoors) {
                t.Open();
            }

            foreach(Tile t in Tile.greenDoors) {
                t.Close();
            }

        }
    }
}
