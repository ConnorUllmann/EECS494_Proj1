using UnityEngine;
using System.Collections;

public class Wallmaster : Enemy {
    public GameObject roomCenter;

    public Vector3 start_point;
    public Sprite[] wiggle;
	// Use this for initialization
	void Start () {
        state_machine = new StateMachine();
        GoToMiddleOfTile();
        start_point = transform.position;
        state_machine.ChangeState(new StateWallmasterWaiting(this));
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        state_machine.Update();
	}

    public override void OnTriggerEnter(Collider coll) {
        base.OnTriggerEnter(coll);
    }
}

public class StateWallmasterWaiting : State {
    Wallmaster w;

    public StateWallmasterWaiting(Wallmaster _w) {
        w = _w;
        w.transform.position = w.start_point;
    }

    public override void OnUpdate(float time_delta_fraction) {
        if(Utils.AreInSameRoom(w.roomCenter, PlayerControl.S.gameObject)) {
            state_machine.ChangeState(new StateWallmasterChasing(w, PlayerControl.S, w.GetComponent<SpriteRenderer>(), w.wiggle));
        }
    }
}

public class StateWallmasterChasing : State {
    Wallmaster w;
    PlayerControl p;
    Rigidbody rb;
    SpriteRenderer renderer;
    Sprite[] animation;
    float current_frame_index = 0;

    public StateWallmasterChasing(Wallmaster _w, PlayerControl _p, SpriteRenderer _renderer, Sprite[] _animation) {
        w = _w;
        p = _p;
        rb = w.GetComponent<Rigidbody>();
        renderer = _renderer;
        animation = _animation;
    }

    public override void OnUpdate(float time_delta_fraction) {
        Vector3 direction = p.transform.position - w.transform.position;
        rb.velocity = direction.normalized * w.speed_max;

        var v = 0.1f * time_delta_fraction * rb.velocity.magnitude / w.speed_max;
        current_frame_index += v;
        while (current_frame_index > animation.Length)
            current_frame_index -= animation.Length;
        renderer.sprite = animation[(int)current_frame_index];

        if (!Utils.AreInSameRoom(w.roomCenter, p.gameObject)) {
            state_machine.ChangeState(new StateWallmasterWaiting(w));
        }
    }
}
