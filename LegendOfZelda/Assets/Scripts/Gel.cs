using UnityEngine;
using System.Collections;

public class Gel : Enemy
{

    public Sprite[] stand;


    // Use this for initialization
    void Start()
    {
        GoToMiddleOfTile();
        speed_max = 3;
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateGelStopped(this));
    }

    public override void Update()
    {
        base.Update();
        state_machine.Update();
        if (state_machine.IsFinished())
            state_machine.ChangeState(new StateGelNormal(this, GetComponent<SpriteRenderer>(), stand));
    }

    public override void OnTriggerEnter(Collider coll)
    {
        base.OnTriggerEnter(coll);
    }
}

public class StateGelNormal : State
{
    Gel p;

    SpriteRenderer renderer;
    Sprite[] animation;
    int animation_length;
    float animation_progression;
    float animation_start_time;

    float current_frame_index = 0;

    private Vector3 nextCell;
    private Vector3 toNextCellStart;

    public StateGelNormal(Gel _p, SpriteRenderer _renderer, Sprite[] _animation)
    {
        p = _p;
        renderer = _renderer;
        animation = _animation;
        animation_length = animation.Length;
        
        GoToRandomCell();

        if (this.animation_length <= 0)
            Debug.LogError("Empty animation submitted to state machine!");
    }

    private void GoToRandomCell()
    {
        var pos = new Vector3(Mathf.Round(p.transform.position.x), Mathf.Round(p.transform.position.y), p.transform.position.z);
        Vector3 speed;
        Tile tile;
        BoxCollider collider;
        do
        {
            speed = Utils.RandomDirection4();
            tile = Tile.GetTile(pos + speed);
            collider = tile.gameObject.GetComponent<BoxCollider>();
        }
        while (Utils.CollidingWithAnyEdge(pos + speed) || (tile != null && collider != null && collider.enabled));

        p.GetComponent<Rigidbody>().velocity = p.speed_max * speed;
        nextCell = pos + speed;
        toNextCellStart = speed;
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if (this.animation_length <= 0)
        {
            Debug.LogError("Empty animation submitted to state machine!");
            return;
        }

        var pos = p.transform.position;

        if (Utils.Dot(nextCell - pos, toNextCellStart) <= 0)//Mathf.Abs(p.transform.position.x - nextCell.x) <= 0.1f && Mathf.Abs(p.transform.position.y - nextCell.y) <= 0.1f)
        {
            pos = p.transform.position = nextCell;
            renderer.sprite = animation[0];
            state_machine.ChangeState(new StateGelStopped(p));
            return;
        }
        
        //Debug.DrawLine(p.transform.position, nextCell);

        // Modulus is necessary so we don't overshoot the length of the animation
        var v = 0.1f * time_delta_fraction;
        current_frame_index += v;
        while (current_frame_index > animation_length)
            current_frame_index -= animation_length;
        renderer.sprite = animation[(int)current_frame_index];

        p.transform.position = pos;

    }
}

public class StateGelStopped : State
{
    Gel p;

    private float pause_seconds = 1;
    private float time_stopped; // Between half and all of pause_seconds.

    public StateGelStopped(Gel _p)
    {
        p = _p;
        time_stopped = (1 + Random.value) / 2 * (72 * pause_seconds);
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        p.GetComponent<Rigidbody>().velocity = new Vector3();

        time_stopped -= time_delta_fraction;
        if (time_stopped <= 0)
            state_machine.ChangeState(new StateGelNormal(p, p.gameObject.GetComponent<SpriteRenderer>(), p.stand));
    }
}