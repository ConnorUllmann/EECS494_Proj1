using UnityEngine;
using System.Collections;

public class Keese : Enemy {

    public Sprite[] flap;


    // Use this for initialization
    void Start ()
    {
        speed_max = 3;
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateKeeseNormal(this, GetComponent<SpriteRenderer>(), flap));
    }

    void Update()
    {
        state_machine.Update();
        if (state_machine.IsFinished())
            state_machine.ChangeState(new StateKeeseNormal(this, GetComponent<SpriteRenderer>(), flap));
    }

    void OnColliderEnter(Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Player":
                HitPlayer();
                break;
        }
    }

    void HitPlayer()
    {

    }
}

public class StateKeeseNormal : State
{
    Keese p;

    SpriteRenderer renderer;
    Sprite[] animation;
    int animation_length;
    float animation_progression;
    float animation_start_time;

    float current_frame_index = 0;
    
    private float normal_seconds = 4; //Seconds of time before liftoff
    private float time_stopped; // Between half and all of normal_seconds.
    private float time_max;

    private Vector3 nextCell;

    public StateKeeseNormal(Keese _p, SpriteRenderer _renderer, Sprite[] _animation)
    {
        p = _p;
        renderer = _renderer;
        animation = _animation;
        animation_length = animation.Length;

        time_max = time_stopped = (1 + Random.value) / 2 * (72 * normal_seconds);

        GoToRandomCell();

        if (this.animation_length <= 0)
            Debug.LogError("Empty animation submitted to state machine!");
    }

    private void GoToRandomCell()
    {
        var speed = Utils.RandomDirection8();
        p.GetComponent<Rigidbody>().velocity = speed;
        nextCell = new Vector3((int)p.transform.position.x + speed.x, (int)p.transform.position.y + speed.y, 0);
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if (this.animation_length <= 0)
        {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }
        

        Rigidbody rb = p.gameObject.GetComponent<Rigidbody>();
        Vector3 rbv = rb.velocity;
        time_stopped -= time_delta_fraction;
        if (time_stopped <= 0)
        {
            state_machine.ChangeState(new StateKeeseNormal(p, p.gameObject.GetComponent<SpriteRenderer>(), p.flap));
        }
        else
        {
            var val = time_stopped / time_max;
            if(val <= 0.25f)
            {
                rbv = val / 0.25f * p.speed_max * rbv.normalized;
            }
            else if(val <= 0.75f)
            {
                rbv = p.speed_max * rbv.normalized;
            }
            else
            {
                rbv = (1 - (val - 0.75f) / 0.25f) * p.speed_max * rbv.normalized;
            }
        }

        if (Utils.CollidingWithTopEdge(p.transform.position))
            rbv.y = -Mathf.Abs(rbv.y);
        if (Utils.CollidingWithBottomEdge(p.transform.position))
            rbv.y = Mathf.Abs(rbv.y);
        if (Utils.CollidingWithLeftEdge(p.transform.position))
            rbv.x = Mathf.Abs(rbv.x);
        if (Utils.CollidingWithRightEdge(p.transform.position))
            rbv.x = -Mathf.Abs(rbv.x);

        rb.velocity = rbv;

        if((p.transform.position - )

        // Modulus is necessary so we don't overshoot the length of the animation
        var v = 0.1f * time_delta_fraction * rb.velocity.magnitude / p.speed_max;
        Debug.Log(v);
        current_frame_index += v;
        while (current_frame_index > animation_length)
            current_frame_index -= animation_length;
        renderer.sprite = animation[(int)current_frame_index];

    }
}

public class StateKeeseStopped : State
{
    Keese p;

    private float pause_seconds = 2;
    private float time_stopped; // Between half and all of pause_seconds.

    public StateKeeseStopped(Keese _p)
    {
        p = _p;
        time_stopped = (1 + Random.value) / 2 * (72 * pause_seconds);
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        time_stopped -= time_delta_fraction;
        if (time_stopped <= 0)
            state_machine.ChangeState(new StateKeeseNormal(p, p.gameObject.GetComponent<SpriteRenderer>(), p.flap));
    }
}