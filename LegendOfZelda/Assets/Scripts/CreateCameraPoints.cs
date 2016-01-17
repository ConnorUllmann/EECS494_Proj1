/*
This class will create CameraPoints for the map you want to use
Place this in the center of the room in the bottom left corner
*/

using UnityEngine;
using System.Collections;

public class CreateCameraPoints : MonoBehaviour {

    public int mapWidthInRooms = 6;
    public int mapHeightInRooms = 6;

    public float roomWidth = 15.8f;
    public float roomHeight = 11.0f;
    public GameObject cameraPointPrefab;

	// This creates 
	void Awake () {
	    for(int i = 0; i < mapWidthInRooms; ++i) {
            for (int j = 0; j < mapHeightInRooms; ++j) {
                GameObject go = Instantiate(cameraPointPrefab) as GameObject;
                float hPos = transform.position.x + (i * roomWidth);
                float vPos = transform.position.y + (j * roomHeight);

                go.transform.position = new Vector3(hPos, vPos, 0);
            }
        }
	}

}
