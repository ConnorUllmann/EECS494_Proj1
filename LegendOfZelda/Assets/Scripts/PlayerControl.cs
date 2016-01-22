using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING, STUNNED};

public class PlayerControl : MonoBehaviour {

    public static PlayerControl S;

    public float walking_velocity = 1.0f;
    public int rupee_count = 0;
    public float health = 3.0f;
    public bool bInvincible = false;
    public float maxInvincibilityTimer = 3.0f;
    private float invincibiltyTimer = 0.0f;

    public float stunTimer = .5f;

    private float roomSize = 16f;
    public float doorMoveOffset = 0.7f;

    public Sprite[] link_run_down;
	public Sprite[] link_run_up;
	public Sprite[] link_run_right;
	public Sprite[] link_run_left;

    public Sprite[] link_run_down_invincible;
    public Sprite[] link_run_up_invincible;
    public Sprite[] link_run_right_invincible;
    public Sprite[] link_run_left_invincible;

    public Sprite[] down_invincible;
    public Sprite[] up_invincible;
    public Sprite[] right_invincible;
    public Sprite[] left_invincible;

    StateMachine animation_state_machine;
	StateMachine control_state_machine;
	
	public EntityState current_state = EntityState.NORMAL;
	public Direction current_direction = Direction.SOUTH;

	public GameObject selected_weapon_prefab_A_button;
    public GameObject selected_weapon_prefab_B_button;
    public bool canBoomerang = true;


	// Use this for initialization
	void Start () {
        if (S != null)
            Debug.LogError("Multiple players!");
        S = this;

        animation_state_machine = new StateMachine();
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
    }
	
	// Update is called once per frame
	void Update () {
        animation_state_machine.Update();
        control_state_machine.Update();
        if (control_state_machine.IsFinished())
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        if(bInvincible) {
            invincibiltyTimer -= Time.deltaTime;
            if(invincibiltyTimer <= 0) {
                bInvincible = false;
                invincibiltyTimer = maxInvincibilityTimer;
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
                break;

            case "Enemy":
                if(!bInvincible) {
                    --health;
                    bInvincible = true;

                    current_state = EntityState.STUNNED;
                    control_state_machine.ChangeState(new StateLinkStunned(this, stunTimer));
                }

                break;
            case "Door":
                MoveToNextRoom();
                break;
        }
    }

    void MoveToNextRoom() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4.0f);
        GameObject nextDoor = new GameObject();
        GameObject thisDoor = new GameObject();

        List<GameObject> enemyList = new List<GameObject>();
        foreach (Collider c in hitColliders) {
            if (c.gameObject.tag == "Door" && Vector3.Distance(transform.position, c.transform.position) > 2.0f && Vector3.Distance(transform.position, c.transform.position) < 5.0f) {
                nextDoor = c.gameObject;
            }
            else if(c.gameObject.tag == "Door" && Vector3.Distance(transform.position, c.transform.position) < 2.0f) {
                thisDoor = c.gameObject;
            }
        }


        Vector3 directionOffset = nextDoor.transform.position - thisDoor.transform.position;

        Vector3 newPos = nextDoor.transform.position + ((directionOffset.normalized) * doorMoveOffset);
        transform.position = newPos;

        pauseCurrentRoom(2.0f);
    }

    public void pauseCurrentRoom(float pauseDuration) {
        control_state_machine.ChangeState(new StateLinkStunned(this, pauseDuration));
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, roomSize);
        foreach(Collider c in hitColliders) {
            if (c.gameObject.tag == "Enemy") {
                c.gameObject.GetComponent<Enemy>().state_machine.ChangeState(new StateEnemyStunned(c.gameObject.GetComponent<Enemy>(), pauseDuration));
            }
        }
    }
}
