using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomClearController : MonoBehaviour {
    public GameObject droppedItemPrefab;
    public bool openSealedDoors;

    private List<GameObject> enemiesInRoom = new List<GameObject>();

    void Awake() {
        GameObject[] allGameObject = FindObjectsOfType<GameObject>();
        foreach(GameObject go in allGameObject) {
            if (((go.tag == "Enemy" || go.tag == "Wallmaster") && !go.name.Contains("Blade")) && Utils.AreInSameRoom(gameObject, go))
                enemiesInRoom.Add(go);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bool allAreDead = true;
        foreach(GameObject go in enemiesInRoom) {
            if(go != null) {
                allAreDead = false;
            }
        }


        if(allAreDead) {
            if (droppedItemPrefab != null) {
                GameObject temp = Instantiate(droppedItemPrefab, transform.position, new Quaternion()) as GameObject;
            }

            if(openSealedDoors) {
                UnsealDoor();
            }

            gameObject.SetActive(false);
        }
	}

    public static void UnsealDoor() {
        foreach(Tile t in Tile.sealedDoors) {
            if(Utils.AreInSameRoom(t.gameObject, PlayerControl.S.gameObject)) {
                t.Open();
            }
        }
    }
}
