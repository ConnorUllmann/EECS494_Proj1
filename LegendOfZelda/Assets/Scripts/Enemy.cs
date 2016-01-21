using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public Direction current_direction = Direction.SOUTH;
    public EntityState current_state = EntityState.NORMAL;

    public StateMachine state_machine;
    public float speed_max;
    public float health = 1.0f;

    // Use this for initialization
    void Start()
    {
    }

    public void GoToMiddleOfTile()
    {
        var temp = transform.position;
        temp.x = (int)Mathf.Round(temp.x);
        temp.y = (int)Mathf.Round(temp.y);
        transform.position = temp;
    }

    // Update is called once per frame
    public virtual void Update()
    {
    	if(health <=0) {
            Destroy(this.gameObject);
        }
    }


    public virtual void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag == "Weapon") {
            --health;
        }
    }
}
