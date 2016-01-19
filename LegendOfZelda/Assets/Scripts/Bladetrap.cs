using UnityEngine;
using System.Collections;

public class Bladetrap : Enemy
{
    // Use this for initialization
    void Start()
    {
        GoToMiddleOfTile();
        state_machine = new StateMachine();
        state_machine.ChangeState(new StateBladetrapNormal(this));
    }

    public override void Update()
    {
        //base.Update();
        state_machine.Update();
        if (state_machine.IsFinished())
            state_machine.ChangeState(new StateBladetrapNormal(this));
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

public class StateBladetrapNormal : State
{
    Bladetrap p;

    private float triggerWidth = 0.8f;

    public StateBladetrapNormal(Bladetrap _p)
    {
        p = _p;
        p.GetComponent<Rigidbody>().velocity = new Vector3();
        p.GoToMiddleOfTile();
        Debug.Log("Stopped!");
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if (PlayerControl.S != null &&
            (Mathf.Abs(p.transform.position.x - PlayerControl.S.transform.position.x) <= triggerWidth ||
             Mathf.Abs(p.transform.position.y - PlayerControl.S.transform.position.y) <= triggerWidth))
        {
            var dir = PlayerControl.S.transform.position - p.transform.position;
            if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir.y = 0;
            else
                dir.x = 0;
            state_machine.ChangeState(new StateBladetrapMoving(p, dir.normalized, true));
        }
    }
}

public class StateBladetrapMoving : State
{
    Bladetrap p;

    private float speed;
    private Vector3 dir;
    private Vector3 nextCell;
    private bool leaveState = false;
    private bool fast = false;

    public StateBladetrapMoving(Bladetrap _p, Vector3 _dir, bool _fast)
    {
        p = _p;
        dir = _dir.normalized;

        fast = _fast;
        speed = fast ? 5f : 2f;

        nextCell = new Vector3((int)p.transform.position.x + dir.x, (int)p.transform.position.y + dir.y);
        if (Utils.CollidingWithAnyWall(nextCell) || Tile.Solid(nextCell))
        {
            leaveState = true;
        }
        Debug.Log("Start moving! " + dir);
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if(leaveState)
        {
            if(fast)
                state_machine.ChangeState(new StateBladetrapMoving(p, -dir, false));
            else
                state_machine.ChangeState(new StateBladetrapNormal(p));
            return;
        }

        p.gameObject.GetComponent<Rigidbody>().velocity = dir * speed;

        if ((Mathf.Abs(nextCell.x - p.transform.position.x) < 0.1f && dir.x != 0.0f) ||
            (Mathf.Abs(nextCell.y - p.transform.position.y) < 0.1f && dir.y != 0.0f))
        {
            nextCell = new Vector3(nextCell.x + dir.x, nextCell.y + dir.y);
            if (Utils.CollidingWithAnyWall(nextCell) || Tile.Solid(nextCell))
            {
                GoToNextState();
                return;
            }
        }

        if(p.gameObject.GetComponent<Rigidbody>().velocity.magnitude <= 0.01f)
        {
            return;
        }
    }

    private void GoToNextState()
    {
        if (fast)
            state_machine.ChangeState(new StateBladetrapMoving(p, -dir, false));
        else
            state_machine.ChangeState(new StateBladetrapNormal(p));
    }
}