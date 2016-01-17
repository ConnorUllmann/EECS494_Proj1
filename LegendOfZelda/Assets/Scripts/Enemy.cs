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
        temp.x = (int)temp.x + 0.5f;
        temp.y = (int)temp.y + 0.5f;
        transform.position = temp;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
