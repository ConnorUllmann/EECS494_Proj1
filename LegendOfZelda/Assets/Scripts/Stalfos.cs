using UnityEngine;
using System.Collections;

public class Stalfos : Enemy {
    public Sprite[] walk;
    public float maxWalkTime = 4.0f; //The longest time it will go without randomizing direction again (in seconds)
    public float minWalkTime = 1.0f; //The shortest time it will go before randomizing direction again

    private float cooldownTimer;

	// Use this for initialization
	void Start () {
        GoToMiddleOfTile();
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateStalfosNormal(this, GetComponent<SpriteRenderer>(), walk));
        cooldownTimer = (Random.value * maxWalkTime) + minWalkTime;
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        cooldownTimer -= Time.deltaTime;
        if(cooldownTimer <= 0) {
            state_machine.ChangeState(new StateStalfosNormal(this, GetComponent<SpriteRenderer>(), walk));
            cooldownTimer = (Random.value * maxWalkTime) + minWalkTime;
        }

        state_machine.Update();
	}

}

public class StateStalfosNormal : State {
    Stalfos s;
    SpriteRenderer renderer;
    Sprite[] animation;
    Rigidbody rb;
    float current_frame_index = 0;

    Vector3 direction;
    Vector3 nextCell = Vector3.zero;

    public StateStalfosNormal(Stalfos _s, SpriteRenderer _renderer, Sprite[] _animation) {
        s = _s;
        rb = s.GetComponent<Rigidbody>();
        renderer = _renderer;
        animation = _animation;

        s.GoToMiddleOfTile();

        do {
            direction = Utils.RandomDirection4();
            nextCell = new Vector3((int)s.transform.position.x + (direction.x * s.speed_max), (int)s.transform.position.y + (direction.y * s.speed_max), 0);
        } while (Tile.Unwalkable(nextCell) || Utils.CollidingWithAnyWall(nextCell));
        

    }

    public override void OnUpdate(float time_delta_fraction) {
        rb.velocity = direction * s.speed_max;

        var v = 0.1f * time_delta_fraction * rb.velocity.magnitude / s.speed_max;
        current_frame_index += v;
        while (current_frame_index > animation.Length)
            current_frame_index -= animation.Length;
        renderer.sprite = animation[(int)current_frame_index];
        nextCell = new Vector3((int)s.transform.position.x + (direction.x * s.speed_max), (int)s.transform.position.y + (direction.y * s.speed_max), 0);
        if(Tile.Unwalkable(nextCell) || Utils.CollidingWithAnyWall(nextCell)) {
            state_machine.ChangeState(new StateStalfosNormal(s, s.GetComponent<SpriteRenderer>(), animation));
        }
    }
}
