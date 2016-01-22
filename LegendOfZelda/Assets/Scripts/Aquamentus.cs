using UnityEngine;
using System.Collections;

public class Aquamentus : Enemy
{

    public Sprite[] walk;
    public AquamentusShot shotPrefab;
    public float bulletSpeed = 10.0f;

    public float stunTimerMax = 20.0f;
    public float stunTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        health = 3.0f;
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateAquamentusNormal(this, GetComponent<SpriteRenderer>(), walk));
    }

    public override void Update()
    {
        base.Update();
        state_machine.Update();
        if (state_machine.IsFinished())
            state_machine.ChangeState(new StateAquamentusNormal(this, GetComponent<SpriteRenderer>(), walk));
        base.Update();
    }

    public override void OnTriggerEnter(Collider coll)
    {
        base.OnTriggerEnter(coll);
    }

    public void Shoot()
    {
        int shots = 3;
        for (int i = 0; i < shots; i++)
        {
            AquamentusShot shot = Instantiate<AquamentusShot>(shotPrefab);
            shot.transform.position = transform.position + new Vector3(0, 0.25f, 0);
            var v = (PlayerControl.S.transform.position - shot.transform.position);
            var v_n = new Vector3(bulletSpeed * Mathf.Sign(v.x), (v.y / Mathf.Abs(v.x) + (i * 1.0f / (shots - 1) * 2 - 1) * 0.333f) * bulletSpeed, 0);
            shot.GetComponent<Rigidbody>().velocity = v_n;
        }
    }

    public override void Hit(Collider coll = null)
    {
        if (stunTimer <= 0)
        {
            base.Hit(coll);
            stunTimer = stunTimerMax;
        }
    }
}

public class StateAquamentusNormal : State
{
    Aquamentus p;

    SpriteRenderer renderer;
    Sprite[] animation;
    int animation_length;
    float animation_progression;
    float animation_start_time;

    float current_frame_index = 0;


    private bool canChangeDirection = true;

    private float waitNextShotMax = 180f;
    private float waitNextShot = 60f;

    public StateAquamentusNormal(Aquamentus _p, SpriteRenderer _renderer, Sprite[] _animation)
    {
        p = _p;
        renderer = _renderer;
        animation = _animation;
        animation_length = animation.Length;

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

        if(p.stunTimer > 0)
        {
            p.stunTimer -= time_delta_fraction;
            p.GetComponent<SpriteRenderer>().color = Mathf.Floor(p.stunTimer * p.stunTimer / 500) % 2 == 0  ? new Color(1, 0.5f, 0.5f) : Color.yellow;
        }
        else
        {
            p.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if(waitNextShot > 0)
        {
            waitNextShot -= time_delta_fraction;
            if(waitNextShot <= 0 && PlayerControl.S.transform.position.x < p.transform.position.x)
            {
                p.Shoot();
                waitNextShot = waitNextShotMax * (Mathf.Floor(1 + Random.value * 1.33f));
            }
        }

        var pos = p.transform.position;

        Rigidbody rb = p.gameObject.GetComponent<Rigidbody>();
        Vector3 rbv = rb.velocity;

        if (p.transform.position.x < 73)
            rbv.x = Mathf.Abs(rbv.x);
        else if (p.transform.position.x > 77)
            rbv.x = -Mathf.Abs(rbv.x);
        else if (Mathf.Abs(Mathf.Round(p.transform.position.x * 2) / 2 - p.transform.position.x) <= 0.1f)
        {
            if (canChangeDirection)
            {
                rbv.x = Mathf.Sign(Random.value * 2 - 1) * p.speed_max;
                canChangeDirection = false;
            }
        }
        else
        {
            canChangeDirection = true;
        }

        rb.velocity = rbv;

        // Modulus is necessary so we don't overshoot the length of the animation
        var v = 0.05f * time_delta_fraction;
        current_frame_index += v;
        while (current_frame_index > animation_length)
            current_frame_index -= animation_length;
        renderer.sprite = animation[(int)current_frame_index];

        p.transform.position = pos;

    }
}