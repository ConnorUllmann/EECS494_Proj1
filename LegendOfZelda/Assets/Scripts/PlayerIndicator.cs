using UnityEngine;
using System.Collections;

public class PlayerIndicator : MonoBehaviour {
    public Vector2 MapSize;
    public Vector2 NumberOfRooms;

    private Vector2 RoomSize;

    private Vector3 oldPos;
    private RectTransform rt;

	// Use this for initialization
	void Start () {
        rt = GetComponent<RectTransform>();
        oldPos = new Vector3(Utils.GetRoomI(PlayerControl.S.transform.position.x), Utils.GetRoomJ(PlayerControl.S.transform.position.y), 0);
        RoomSize = new Vector2(MapSize.x / NumberOfRooms.x, MapSize.y / NumberOfRooms.y);

        MoveIndicator(oldPos);
	}
	
	// Update is called once per frame
	void Update () {
	    Vector3 newPos = new Vector3(Utils.GetRoomI(PlayerControl.S.transform.position.x), Utils.GetRoomJ(PlayerControl.S.transform.position.y), 0);
        if(newPos != oldPos) {
            oldPos = newPos;
            MoveIndicator(oldPos);
        }
    }

    void MoveIndicator(Vector3 RoomCoordinates) {
        rt.anchoredPosition = new Vector2((RoomCoordinates.x + 0.5f) * RoomSize.x, (RoomCoordinates.y + 0.5f) * RoomSize.y);
    }
}
