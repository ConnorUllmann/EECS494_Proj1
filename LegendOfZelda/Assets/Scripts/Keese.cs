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

    void EnterNewCell()
    {
        GetComponent<Rigidbody>().velocity = RandomDirection();
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

    int fps_max = 3;
    int fps;
    
    private float normal_seconds = 4; //Seconds of time before liftoff
    private float time_stopped; // Between half and all of normal_seconds.

    public StateKeeseNormal(Keese _p, SpriteRenderer _renderer, Sprite[] _animation)
    {
        p = _p;
        renderer = _renderer;
        animation = _animation;
        animation_length = animation.Length;

        time_stopped = (1 + Random.value) / 2 * (72 * normal_seconds);

        var a = Random.value * 2 * Mathf.PI;
        p.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
        if (this.animation_length <= 0)
            Debug.LogError("Empty animation submitted to state machine!");
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if (this.animation_length <= 0)
        {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }

        Debug.Log("Normal! " + time_stopped);
        time_stopped -= time_delta_fraction;

        Rigidbody rb = p.gameObject.GetComponent<Rigidbody>();
        Vector3 rbv = rb.velocity;
        if (time_stopped <= 0)
        {
            Debug.Log("Slow!");
            var n = Mathf.Max(1 - 5 / rbv.magnitude, 0);
            rbv.x *= n;
            rbv.y *= n;
            rbv.z *= n;

            if (rbv.magnitude <= 0)
                state_machine.ChangeState(new StateKeeseNormal(p, p.gameObject.GetComponent<SpriteRenderer>(), p.flap));
        }
        else
        {
            if (rbv.magnitude > 0)
            {
                var n = Mathf.Max(1 + 5 / rbv.magnitude, 0);
                rbv.x *= n;
                rbv.y *= n;
                rbv.z *= n;
            }
            if (rbv.magnitude >= p.speed_max)
            {
                var n = p.speed_max / rbv.magnitude;
                rbv.x *= n;
                rbv.y *= n;
                rbv.z *= n;
            }
        }
        rb.velocity = rbv;

        fps = 6;// (int)(fps_max * rb.velocity.magnitude / 100);
        // Modulus is necessary so we don't overshoot the length of the animation.
        int current_frame_index = ((int)((Time.time - animation_start_time) / (1.0 / fps)) % animation_length);
        renderer.sprite = animation[current_frame_index];

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