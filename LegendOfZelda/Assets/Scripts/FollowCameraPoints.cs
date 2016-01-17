using UnityEngine;
using System.Collections.Generic;

public class FollowCameraPoints : MonoBehaviour {
    public static FollowCameraPoints S;

    public Transform cameraPoint;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float easing = 0.1f;

	// Use this for initialization
	void Awake () {
        S = this;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 p0, p1;
        p0 = transform.position;
        p1 = CameraPoints.GetClosestCameraPoint().transform.position + offset;

        transform.position = (1 - easing) * p0 + easing * p1;
    }
}

