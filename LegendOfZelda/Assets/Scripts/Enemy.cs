﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Direction current_direction = Direction.SOUTH;
    public EntityState current_state = EntityState.NORMAL;

    public StateMachine state_machine;
    public float speed_max;

    // Use this for initialization
    void Start()
    {
    }

    public void GoToMiddleOfTile()
    {
        var temp = transform.position;
        temp.x = (int)temp.x;
        temp.y = (int)temp.y;
        transform.position = temp;
    }

    // Update is called once per frame
    void Update()
    {

    }

}