using UnityEngine;
using System.Collections;

public class Keese : Enemy {

    public Sprite[] flap;


    // Use this for initialization
    void Start ()
    {
        GoToMiddleOfTile();
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
    
    private float normal_seconds = 8; //Seconds of time before liftoff
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

    private void GoToCell(Vector3 speed)
    {
        speed.Normalize();
        speed.x = Mathf.Round(speed.x);
        speed.y = Mathf.Round(speed.y);
        p.GetComponent<Rigidbody>().velocity = speed;
        nextCell = new Vector3((int)p.transform.position.x + speed.x, (int)p.transform.position.y + speed.y, 0);

    }
    private void GoToRandomCell()
    {
        var pos = new Vector3((int)p.transform.position.x, (int)p.transform.position.y, p.transform.position.z);
        var speed = Utils.RandomDirection8();
        while (Utils.CollidingWithAnyEdge(pos + speed))
            speed = Utils.RandomDirection8();
        GoToCell(speed);
    }
    private void SetNextCellToMoveTo()
    {
        if(Random.value < 0.5)
        {
            GoToCell(p.GetComponent<Rigidbody>().velocity.normalized);
        }
        else
        {
            GoToRandomCell();
        }
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if (this.animation_length <= 0)
        {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }

        var pos = p.transform.position;


        Rigidbody rb = p.gameObject.GetComponent<Rigidbody>();
        Vector3 rbv = rb.velocity;
        time_stopped -= time_delta_fraction;
        if (time_stopped <= 0)
        {
            state_machine.ChangeState(new StateKeeseStopped(p));
        }
        else
        {
            var val = time_stopped / time_max;
            if (val <= 0.25f)
                rbv = val / 0.25f * p.speed_max * rbv.normalized;
            else if (val <= 0.75f)
                rbv = p.speed_max * rbv.normalized;
            else
                rbv = (1 - (val - 0.75f) / 0.25f) * p.speed_max * rbv.normalized;
        }

        if (Utils.CollidingWithTopEdge(p.transform.position))
        {
            rbv.y = -Mathf.Abs(rbv.y);
            GoToRandomCell();
        }
        if (Utils.CollidingWithBottomEdge(p.transform.position))
        {
            rbv.y = Mathf.Abs(rbv.y);
            GoToRandomCell();
        }
        if (Utils.CollidingWithLeftEdge(p.transform.position))
        {
            rbv.x = Mathf.Abs(rbv.x);
            GoToRandomCell();
        }
        if (Utils.CollidingWithRightEdge(p.transform.position))
        {
            rbv.x = -Mathf.Abs(rbv.x);
            GoToRandomCell();
        }

        rb.velocity = rbv;

        
        if (Mathf.Abs(p.transform.position.x - nextCell.x) <= 0.4f && Mathf.Abs(p.transform.position.y - nextCell.y) <= 0.4f)
        {
            //pos = nextCell;
            SetNextCellToMoveTo();
        }

        // Modulus is necessary so we don't overshoot the length of the animation
        var v = 0.1f * time_delta_fraction * rb.velocity.magnitude / p.speed_max;
        current_frame_index += v;
        while (current_frame_index > animation_length)
            current_frame_index -= animation_length;
        renderer.sprite = animation[(int)current_frame_index];

        p.transform.position = pos;

    }
}

public class StateKeeseStopped : State
{
    Keese p;

    private float pause_seconds = 3;
    private float time_stopped; // Between half and all of pause_seconds.

    public StateKeeseStopped(Keese _p)
    {
        p = _p;
        time_stopped = (1 + Random.value) / 2 * (72 * pause_seconds);
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        p.GetComponent<Rigidbody>().velocity = new Vector3();

        time_stopped -= time_delta_fraction;
        if (time_stopped <= 0)
            state_machine.ChangeState(new StateKeeseNormal(p, p.gameObject.GetComponent<SpriteRenderer>(), p.flap));
    }
}