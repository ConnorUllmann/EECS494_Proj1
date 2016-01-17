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
    void Update() {
        animation_state_machine.Update();
        control_state_machine.Update();
        if (control_state_machine.IsFinished())
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        

        /*Vector3[] pt = 
        {
            new Vector3(Utils.GetRoomX(transform.position.x), Utils.GetRoomY(transform.position.y), -5),
            new Vector3(Utils.GetRoomX(transform.position.x) + Utils.roomSize.x, Utils.GetRoomY(transform.position.y), -5),
            new Vector3(Utils.GetRoomX(transform.position.x) + Utils.roomSize.x, Utils.GetRoomY(transform.position.y) + Utils.roomSize.y, -5),
            new Vector3(Utils.GetRoomX(transform.position.x), Utils.GetRoomY(transform.position.y) + Utils.roomSize.y, -5)
        };

        for (int i = 0; i < 4; i++)
        {
            int o = (i + 1) % 4;
            Debug.DrawLine(pt[i], pt[o], i == 0 ? Color.blue : (i == 1 ? Color.green : Color.white));
        }*/
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
