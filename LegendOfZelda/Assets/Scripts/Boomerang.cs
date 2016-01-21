using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour {
    public float speed = 5.0f;

    private StateMachine state_machine = new StateMachine();

    // Use this for initialization
    void Start () {
        state_machine.ChangeState(new StateBoomerangForward(PlayerControl.S, this, speed));
	}
	
	// Update is called once per frame
	void Update () {
        state_machine.Update();
	}

    void OnCollisionEnter(Collision coll) {
        print(coll.gameObject.name);
        state_machine.ChangeState(new StateBoomerangReturning(PlayerControl.S, this, speed));
    }

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag == "Enemy") {
            state_machine.ChangeState(new StateBoomerangReturning(PlayerControl.S, this, speed));
        }
        if(coll.gameObject.tag == "Hero") {
            Destroy(this.gameObject);
        }
    }

}

public class StateBoomerangForward : State {
    Vector3 direction_offset;
    PlayerControl p;
    Boomerang b;
    float speed;

    public StateBoomerangForward(PlayerControl _p, Boomerang _b, float _speed) {
        b = _b;
        speed = _speed;

        p = _p;
        if (p.current_direction == Direction.NORTH) {
            direction_offset = new Vector3(0, 1, 0);
        } else if (p.current_direction == Direction.EAST) {
            direction_offset = new Vector3(1, 0, 0);
        } else if (p.current_direction == Direction.SOUTH) {
            direction_offset = new Vector3(0, -1, 0);;
        } else if (p.current_direction == Direction.WEST) {
            direction_offset = new Vector3(-1, 0, 0);
        }

        b.GetComponent<Rigidbody>().velocity = direction_offset * speed;
    }

    public override void OnUpdate(float time_delta_fraction) {


    }
}

public class StateBoomerangReturning : State {

    PlayerControl p;
    Boomerang b;
    float speed;

    public StateBoomerangReturning(PlayerControl _p, Boomerang _b, float _speed) {
        b = _b;
        speed = _speed;
        p = _p;

        b.GetComponent<BoxCollider>().isTrigger = true;
    }

    public override void OnUpdate(float time_delta_fraction) {
        Vector3 orientation = p.transform.position - b.transform.position;
        b.GetComponent<Rigidbody>().velocity = orientation.normalized * speed;
        
    }

}
