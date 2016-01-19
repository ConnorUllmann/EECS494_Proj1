/*
This is to be attached to a Gameobject that sits at the center of each room.
This will include a function to find which camerapoint is closest to the player.
*/

using UnityEngine;
using System.Collections.Generic; //list
using System.Collections;

public class CameraPoints : MonoBehaviour {

    //static vector to be populated with camerapoints
    public static List<GameObject> cameraPoints = new List<GameObject>();

    //Add this camerapoint to the list when it becomes awake
	void Awake() {
        cameraPoints.Add(gameObject);
    }
	
    //Finds the CameraPoint nearest the player
	public static GameObject GetClosestCameraPoint() {

        GameObject closestCameraPoint = cameraPoints[0];
        float minDistance = 999999999f;

        foreach(GameObject cpoint in cameraPoints) {
            float distance = Vector3.Distance(cpoint.transform.position, PlayerControl.S.transform.position);
            if(distance < minDistance) {
                closestCameraPoint = cpoint;
                minDistance = distance;
            }
        }

        return closestCameraPoint;
    }
}
