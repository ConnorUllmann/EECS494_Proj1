using UnityEngine;
using System.Collections;

public class Bladetrap : Enemy
{
    public Vector3 startPos;

    // Use this for initialization
    void Start()
    {
        startPos = new Vector3((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), 0);
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
}

public class StateBladetrapNormal : State
{
    Bladetrap p;

    private float triggerWidth = 0.8f;

    public StateBladetrapNormal(Bladetrap _p)
    {
        p = _p;
        p.GetComponent<Rigidbody>().velocity = new Vector3();
        p.transform.position = p.startPos;
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

        nextCell = new Vector3(Mathf.Round(p.transform.position.x), Mathf.Round(p.transform.position.y)) + dir;
        if (Utils.CollidingWithAnyWall(nextCell) || Tile.Unwalkable(nextCell))
        {
            leaveState = true;
        }
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        if(leaveState)
        {
            Debug.Log("[0]");
            GoToNextState();
            return;
        }

        p.gameObject.GetComponent<Rigidbody>().velocity = dir * speed;

        Debug.DrawLine(p.transform.position, nextCell);

        if ((Mathf.Abs(nextCell.x - p.transform.position.x) < 0.1f && dir.x != 0.0f) ||
            (Mathf.Abs(nextCell.y - p.transform.position.y) < 0.1f && dir.y != 0.0f))
        {
            if(nextCell == p.startPos && !fast)
            {
                GoToNextState();
                return;
            }
            nextCell = new Vector3(nextCell.x + dir.x, nextCell.y + dir.y);
            if (Utils.CollidingWithAnyWall(nextCell) || Tile.Unwalkable(nextCell))
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