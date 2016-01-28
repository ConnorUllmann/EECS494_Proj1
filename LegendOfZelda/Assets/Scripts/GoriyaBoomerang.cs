using UnityEngine;
using System.Collections;

public class GoriyaBoomerang : MonoBehaviour {
    public float speed = 5.0f;
    public Goriya g;

    private StateMachine state_machine = new StateMachine();

    // Use this for initialization
    void Start () {
        state_machine.ChangeState(new StateGoriyaBoomerangForward(g, this, speed));
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (g == null) {
            Destroy(this.gameObject);
            return;
        }
        state_machine.Update();
	}

    void OnCollisionEnter(Collision coll) {
        state_machine.ChangeState(new StateGoriyaBoomerangReturning(g, this, speed));
    }

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.tag == "Enemy") {
            state_machine.ChangeState(new StateGoriyaBoomerangReturning(g, this, speed));
        }

        if(g == null) {
            return;
        }
        if(coll.GetComponent<Goriya>() == g) {
            g.boomerangReturned = true;
            Destroy(this.gameObject);
        }
    }

}

public class StateGoriyaBoomerangForward : State {
    Vector3 direction_offset;
    Goriya p;
    GoriyaBoomerang b;
    float speed;

    public StateGoriyaBoomerangForward(Goriya _p, GoriyaBoomerang _b, float _speed) {
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
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 0, 720) * Time.deltaTime);
        Rigidbody rb = b.gameObject.GetComponent<Rigidbody>();
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}

public class StateGoriyaBoomerangReturning : State {

    Goriya p;
    GoriyaBoomerang b;
    float speed;
    GameObject pickedUpObject;
    //Changed this

    public StateGoriyaBoomerangReturning(Goriya _p, GoriyaBoomerang _b, float _speed, GameObject _pickedUpObject = null) {
        b = _b;
        speed = _speed;
        p = _p;
        pickedUpObject = _pickedUpObject;

        b.GetComponent<BoxCollider>().isTrigger = true;
    }

    public override void OnUpdate(float time_delta_fraction) {
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 0, 720) * Time.deltaTime);
        Rigidbody rb = b.gameObject.GetComponent<Rigidbody>();
        rb.MoveRotation(rb.rotation * deltaRotation);

        Vector3 orientation = p.transform.position - b.transform.position;
        rb.velocity = orientation.normalized * speed;

        if(pickedUpObject != null) {
            pickedUpObject.transform.position = b.transform.position;
        }
        
    }

}
