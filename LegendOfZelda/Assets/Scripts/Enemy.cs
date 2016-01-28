using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    public Direction current_direction = Direction.SOUTH;
    public EntityState current_state = EntityState.NORMAL;

    public StateMachine state_machine;
    public float speed_max;
    public float health = 1.0f;
    public bool damagedByBoomerang = false;
    public bool IsWallmaster = false;

    // Use this for initialization
    void Start()
    {
    }

    public void GoToMiddleOfTile()
    {
        var temp = transform.position;
        temp.x = (int)Mathf.Round(temp.x);
        temp.y = (int)Mathf.Round(temp.y);
        transform.position = temp;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (health <=0) {
            Destroy(this.gameObject);
        }

        /*if(!Utils.AreInSameRoom(gameObject, PlayerControl.S.gameObject) && !IsWallmaster) { 
            state_machine.ChangeState(new StateEnemyStunned(this, 2.0f));
        }*/
    }

    public virtual void Hit(Collider coll=null)
    {
        health--;
    }

    public virtual void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Weapon")
        {
            coll.gameObject.GetComponent<Sword>().Hit();
            Hit(coll);
        }
        else if ((coll.gameObject.tag == "Boomerang" && damagedByBoomerang) || coll.gameObject.tag == "Arrow" || coll.gameObject.tag == "SwordExplosive")
        {
            Hit(coll);
        }
        if(coll.gameObject.tag == "Boomerang" && !damagedByBoomerang) {
            state_machine.ChangeState(new StateEnemyStunned(this, 1.0f));
        }
    }
}

public class StateEnemyStunned : State {
    Enemy pc;
    float cooldown;

    public StateEnemyStunned(Enemy _pc, float _cooldown) {
        pc = _pc;
        cooldown = _cooldown;
    }

    public override void OnStart() {
        pc.current_state = EntityState.STUNNED;
        pc.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public override void OnUpdate(float time_delta_fraction) {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
            ConcludeState();

    }

    public override void OnFinish() {
        pc.current_state = EntityState.NORMAL;
    }

}
