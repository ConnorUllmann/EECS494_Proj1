using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Direction current_direction = Direction.SOUTH;
    public EntityState current_state = EntityState.NORMAL;

    public StateMachine state_machine;
    public float speed_max;

    // Use this for initialization
    void Start()
    {

        var temp = transform.position;
        temp.x = Mathf.Floor(transform.position.x / 16) * 16;
        temp.y = Mathf.Floor(transform.position.y / 16) * 16;
        temp.z = Mathf.Floor(transform.position.z / 16) * 16;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Vector3 RandomDirection()
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
}
