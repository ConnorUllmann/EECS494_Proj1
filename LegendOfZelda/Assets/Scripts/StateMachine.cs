﻿using UnityEngine;

// State Machines are responsible for processing states, notifying them when they're about to begin or conclude, etc.
public class StateMachine
{
	private State _current_state;
	
	public void ChangeState(State new_state)
	{
		if(_current_state != null)
		{
			_current_state.OnFinish();
		}
		
		_current_state = new_state;
		// States sometimes need to reset their machine. 
		// This reference makes that possible.
		_current_state.state_machine = this;
		_current_state.OnStart();
	}
	
	public void Reset()
	{
		if(_current_state != null)
			_current_state.OnFinish();
		_current_state = null;
	}
	
	public void Update()
	{
		if(_current_state != null)
		{
			float time_delta_fraction = Time.deltaTime / (1.0f / Application.targetFrameRate);
			_current_state.OnUpdate(time_delta_fraction);
		}
	}

	public bool IsFinished()
	{
		return _current_state == null;
	}
}

// A State is merely a bundle of behavior listening to specific events, such as...
// OnUpdate -- Fired every frame of the game.
// OnStart -- Fired once when the state is transitioned to.
// OnFinish -- Fired as the state concludes.
// State Constructors often store data that will be used during the execution of the State.
public class State
{
	// A reference to the State Machine processing the state.
	public StateMachine state_machine;
	
	public virtual void OnStart() {}
	public virtual void OnUpdate(float time_delta_fraction) {} // time_delta_fraction is a float near 1.0 indicating how much more / less time this frame took than expected.
	public virtual void OnFinish() {}
	
	// States may call ConcludeState on themselves to end their processing.
	public void ConcludeState() { state_machine.Reset(); }
}

// A State that takes a renderer and a sprite, and implements idling behavior.
// The state is capable of transitioning to a walking state upon key press.
public class StateIdleWithSprite : State
{
	PlayerControl pc;
	SpriteRenderer renderer;
	Sprite sprite;
	
	public StateIdleWithSprite(PlayerControl pc, SpriteRenderer renderer, Sprite sprite)
	{
		this.pc = pc;
		this.renderer = renderer;
		this.sprite = sprite;
	}
	
	public override void OnStart()
	{
		renderer.sprite = sprite;
	}
	
	public override void OnUpdate(float time_delta_fraction)
	{
		if(pc.current_state == EntityState.ATTACKING)
			return;

		// Transition to walking animations on key press.
		if(Input.GetKeyDown(KeyCode.DownArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_down, 6, KeyCode.DownArrow));
		if(Input.GetKeyDown(KeyCode.UpArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_up, 6, KeyCode.UpArrow));
		if(Input.GetKeyDown(KeyCode.RightArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_right, 6, KeyCode.RightArrow));
		if(Input.GetKeyDown(KeyCode.LeftArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_left, 6, KeyCode.LeftArrow));
	}
}

// A State for playing an animation until a particular key is released.
// Good for animations such as walking.
public class StatePlayAnimationForHeldKey : State
{
	PlayerControl pc;
	SpriteRenderer renderer;
	KeyCode key;
	Sprite[] animation;
	int animation_length;
	float animation_progression;
	float animation_start_time;
	int fps;
	
	public StatePlayAnimationForHeldKey(PlayerControl pc, SpriteRenderer renderer, Sprite[] animation, int fps, KeyCode key)
	{
		this.pc = pc;
		this.renderer = renderer;
		this.key = key;
		this.animation = animation;
		this.animation_length = animation.Length;
		this.fps = fps;
		
		if(this.animation_length <= 0)
			Debug.LogError("Empty animation submitted to state machine!");
	}
	
	public override void OnStart()
	{
		animation_start_time = Time.time;
	}
	
	public override void OnUpdate(float time_delta_fraction)
	{
		if(pc.current_state == EntityState.ATTACKING)
			return;

		if(this.animation_length <= 0)
		{
			Debug.LogError("Empty animation submitted to state machine!");
			return;
		}
		
		// Modulus is necessary so we don't overshoot the length of the animation.
		int current_frame_index = ((int)((Time.time - animation_start_time) / (1.0 / fps)) % animation_length);
		renderer.sprite = animation[current_frame_index];
		
		// If another key is pressed, we need to transition to a different walking animation.
		if(Input.GetKeyDown(KeyCode.DownArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_down, 6, KeyCode.DownArrow));
		else if(Input.GetKeyDown(KeyCode.UpArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_up, 6, KeyCode.UpArrow));
		else if(Input.GetKeyDown(KeyCode.RightArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_right, 6, KeyCode.RightArrow));
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
			state_machine.ChangeState(new StatePlayAnimationForHeldKey(pc, renderer, pc.link_run_left, 6, KeyCode.LeftArrow));
		
		// If we detect the specified key has been released, return to the idle state.
		else if(!Input.GetKey(key))
			state_machine.ChangeState(new StateIdleWithSprite(pc, renderer, animation[1]));
	}
}

public class StateLinkNormalMovement : State
{
    PlayerControl p;
        
    public StateLinkNormalMovement(PlayerControl _p)
    {
        p = _p;
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        float h_input = Input.GetAxis("Horizontal");
        float v_input = Input.GetAxis("Vertical");

        if (h_input != 0.0f)
            v_input = 0;

        p.GetComponent<Rigidbody>().velocity = new Vector3(h_input, v_input, 0) * p.walking_velocity;

        if (h_input > 0.0f)
            p.current_direction = Direction.EAST;
        else if (h_input < 0.0f)
            p.current_direction = Direction.WEST;
        else if (v_input > 0.0f)
            p.current_direction = Direction.NORTH;
        else if (v_input < 0.0f)
            p.current_direction = Direction.SOUTH;

        if (Input.GetKeyDown(KeyCode.Z))
            state_machine.ChangeState(new StateLinkAttack(p, p.selected_weapon_prefab, 15));
    }
}

public class StateLinkAttack : State
{
    PlayerControl p;
    GameObject weapon_prefab;
    GameObject weapon_instance;
    float cooldown = 0.0f;

    public StateLinkAttack(PlayerControl _p, GameObject _weapon_prefab, int _cooldown)
    {
        p = _p;
        weapon_prefab = _weapon_prefab;
        cooldown = _cooldown;
    }

    public override void OnStart()
    {
        p.current_state = EntityState.ATTACKING;

        p.GetComponent<Rigidbody>().velocity = Vector3.zero;

        weapon_instance = MonoBehaviour.Instantiate(weapon_prefab, p.transform.position, Quaternion.identity) as GameObject;

        Vector3 direction_offset = Vector3.zero;
        Vector3 direction_eulerangle = Vector3.zero;

        if(p.current_direction == Direction.NORTH)
        {
            direction_offset = new Vector3(0, 1, 0);
            direction_eulerangle = new Vector3(0, 0, 90);
        }
        else if(p.current_direction == Direction.EAST)
        {
            direction_offset = new Vector3(1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 0);
        }
        else if(p.current_direction == Direction.SOUTH)
        {
            direction_offset = new Vector3(0, -1, 0);
            direction_eulerangle = new Vector3(0, 0, 270);
        }
        else if(p.current_direction == Direction.WEST)
        {
            direction_offset = new Vector3(-1, 0, 0);
            direction_eulerangle = new Vector3(0, 0, 180);
        }

        weapon_instance.transform.position += direction_offset;
        Quaternion new_weapon_rotation = new Quaternion();
        new_weapon_rotation.eulerAngles = direction_eulerangle;
        weapon_instance.transform.rotation = new_weapon_rotation;
    }

    public override void OnUpdate(float time_delta_fraction)
    {
        cooldown -= time_delta_fraction;

        if(cooldown <= 0)
            ConcludeState();
    }

    public override void OnFinish()
    {
        p.current_state = EntityState.NORMAL;
        MonoBehaviour.Destroy(weapon_instance);
    }
}

// Additional recommended states:
// StateDeath
// StateDamaged
// StateWeaponSwing
// StateVictory

// Additional control states:
// LinkNormalMovement.
// LinkStunnedState.