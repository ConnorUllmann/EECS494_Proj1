using UnityEngine;
using System.Collections;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING};

public class PlayerControl : MonoBehaviour {

    public static PlayerControl S;

    public float walking_velocity = 1.0f;
    public int rupee_count = 0;
    public float health = 3.0f;
    public bool bInvincible = false;
    public float invincibilityTimer = 3.0f;

    private float distanceToNextDoor = 3.5f;
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

	public GameObject selected_weapon_prefab;


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
            invincibilityTimer -= Time.deltaTime;
            if(invincibilityTimer <= 0) {
                bInvincible = false;
                invincibilityTimer = 3.0f;
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
                }
                break;
            case "Door":
                MoveToNextRoom();
                break;
        }
    }

    void MoveToNextRoom() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distanceToNextDoor);
        GameObject nextDoor = new GameObject();
        foreach (Collider c in hitColliders) {
            if (c.gameObject.tag == "Door" && Vector3.Distance(transform.position, c.transform.position) > 2.0f) {
                nextDoor = c.gameObject;
            }
        }

        Vector3 directionOffset = nextDoor.transform.position - transform.position;

        Vector3 newPos = nextDoor.transform.position + ((directionOffset / directionOffset.magnitude) * doorMoveOffset);
        transform.position = newPos;
    }
}
