using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

    public static Vector2 roomSize = new Vector2(16, 11);

    public static int GetTileI(float _x) { return (int)_x; }
    public static int GetTileJ(float _y) { return (int)_y; }

    //Returns the x/y indices of the tile at (_x, _y)
    //e.g. GetRoomTileInRoomI(x) = 0 means  x is in the left-most  column of tiles in the room
    //     GetRoomTileInRoomI(x) = 15 means x is in the right-most column of tiles in the room
    public static int GetTileInRoomI(float _x) { return (int)(_x - GetRoomX(_x)); }
    public static int GetTileInRoomJ(float _y) { return (int)(_y - GetRoomY(_y)); }

    public static float GetRoomX(float _x) { return Mathf.FloorToInt(_x / roomSize.x) * roomSize.x - 0.5f; }
    public static float GetRoomY(float _y) { return Mathf.FloorToInt(_y / roomSize.y) * roomSize.y - 0.5f; }
    public static int GetRoomI(float _x) { return Mathf.FloorToInt(_x / roomSize.x); }
    public static int GetRoomJ(float _y) { return Mathf.FloorToInt(_y / roomSize.y); }

    public static bool CollidingWithTopEdge(Vector3 p) { return GetTileInRoomJ(p.y) >= 10; }
    public static bool CollidingWithBottomEdge(Vector3 p) { return GetTileInRoomJ(p.y) <= 0; }
    public static bool CollidingWithLeftEdge(Vector3 p) { return GetTileInRoomI(p.x) <= 0; }
    public static bool CollidingWithRightEdge(Vector3 p) { return GetTileInRoomI(p.x) >= 15; }
    public static bool CollidingWithAnyEdge(Vector3 p) { return CollidingWithTopEdge(p) || CollidingWithBottomEdge(p) || CollidingWithLeftEdge(p) || CollidingWithRightEdge(p); }

    public static bool CollidingWithTopWall(Vector3 p) { return GetTileInRoomJ(p.y) >= 9; }
    public static bool CollidingWithBottomWall(Vector3 p) { return GetTileInRoomJ(p.y) <= 1; }
    public static bool CollidingWithLeftWall(Vector3 p) { return GetTileInRoomI(p.x) <= 1; }
    public static bool CollidingWithRightWall(Vector3 p) { return GetTileInRoomI(p.x) >= 14; }
    public static bool CollidingWithAnyWall(Vector3 p) { return CollidingWithTopWall(p) || CollidingWithBottomWall(p) || CollidingWithLeftWall(p) || CollidingWithRightWall(p); }

    public static float Dot(Vector3 a, Vector3 b) { return a.x * b.x + a.y * b.y + a.z * b.z; }

    //Returns a random vector pointing in one of the four cardinal directions
    public static Vector3 RandomDirection4()
    {
        int v = Mathf.FloorToInt(Random.value * 4) % 4;
        switch (v)
        {
            case 0:
                return new Vector3(1, 0, 0);
            case 1:
                return new Vector3(0, 1, 0);
            case 2:
                return new Vector3(-1, 0, 0);
            case 3:
                return new Vector3(0, -1, 0);
        }
        return new Vector3();
    }

    //Returns the sign of the number (0 = 0)
    public static float Sign(float n)
    {
        return n < 0 ? -1 : (n > 0 ? 1 : 0);
    }

    //Returns a random vector corresponding to one of the 8 directions (both cardinal and diagonal)
    //Note: These are NOT unit vectors.
    public static Vector3 RandomDirection8()
    {
        int v = Mathf.FloorToInt(Random.value * 8);
        switch (v)
        {
            case 0:
                return new Vector3(1, 0, 0);
            case 1:
                return new Vector3(1, 1, 0);
            case 2:
                return new Vector3(0, 1, 0);
            case 3:
                return new Vector3(-1, 1, 0);
            case 4:
                return new Vector3(-1, 0, 0);
            case 5:
                return new Vector3(-1, -1, 0);
            case 6:
                return new Vector3(0, -1, 0);
            case 7:
                return new Vector3(1, -1, 0);
        }
        return new Vector3();
    }

    public static bool AreInSameRoom(GameObject A, GameObject B) {

        return (Mathf.FloorToInt(A.transform.position.x / roomSize.x) == Mathf.FloorToInt(B.transform.position.x / roomSize.x)) &&
                (Mathf.FloorToInt(A.transform.position.y / roomSize.y) == Mathf.FloorToInt(B.transform.position.y / roomSize.y));
    }

    public static Vector3 DirectionToVector(Direction d) {
        switch (d) {
            case Direction.NORTH:
                return new Vector3(0, 1, 0);
            case Direction.EAST:
                return new Vector3(1, 0, 0);
            case Direction.WEST:
                return new Vector3(-1, 0, 0);
            case Direction.SOUTH:
                return new Vector3(0, -1, 0);
        }
        return Vector3.zero;
    }

    public static Vector3 Reflect(Vector3 point, Vector3 normal, Vector3 normal_vec) {
        normal_vec = 2 * Utils.Dot(point - normal, normal_vec) * normal_vec.normalized;
        point = -point;
        normal *= 2;
        return point + normal + normal_vec;
    }
// Use this for initialization
void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
