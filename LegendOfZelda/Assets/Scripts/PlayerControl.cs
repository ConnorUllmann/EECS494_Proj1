using UnityEngine;
using System.Collections;

public enum Direction {NORTH, EAST, SOUTH, WEST, NORTHEAST, NORTHWEST, SOUTHEAST, SOUTHWEST};
public enum EntityState {NORMAL, ATTACKING};

public class PlayerControl : MonoBehaviour {

    public static PlayerControl S;

    public float walking_velocity = 1.0f;
    public int rupee_count = 0;

    public Sprite[] link_run_down;
	public Sprite[] link_run_up;
	public Sprite[] link_run_right;
	public Sprite[] link_run_left;

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
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
                break;
        }
    }
}
